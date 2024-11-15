using UnityEngine;

public class CoinPhysics : MonoBehaviour
{
    public float initialAngularVelocity = 12.0f;    // Initial angular velocity (spin speed)
    public float rollingFriction = 0.05f;           // Rolling friction coefficient (coin resistance)
    public float mass = 0.01f;                      // Mass of the coin (kg, 10g)
    public float gravity = 9.81f;                   // Gravity acceleration (m/s^2)
    public float bounceDamping = 0.4f;              // Bounce damping factor (how much energy is lost after bounce)
    public float tiltDamping = 0.6f;                // Damping for tilt (how quickly the coin comes to rest)
    public float substepInterval = 0.01f;           // Time step for substepping (for more accuracy)
    public float planeHeight = 0.0f;                // Plane height (usually y = 0)

    private float angularVelocity;                  // Angular velocity of the coin
    private float currentAngle;                     // Current angle of rotation
    private float verticalVelocity = 0.0f;          // Vertical velocity for freefall
    private float height = 5.5f;                    // Initial height of the coin
    private float tiltAngle = 1f;                 // Tilt angle of the coin
    private bool hasCollided = false;               // To check if coin has collided with the plane

    void Start()
    {
        // Set the coin’s initial position above the plane at y = 2
        transform.position = new Vector3(transform.position.x, 2.0f, transform.position.z);

        angularVelocity = initialAngularVelocity;
    }

    void Update()
    {
        float deltaTime = Time.deltaTime;
        SimulateCoinMotion(deltaTime);
    }

    void SimulateCoinMotion(float deltaTime)
    {
        // Divide deltaTime into substeps for better precision
        int substeps = Mathf.CeilToInt(deltaTime / substepInterval);
        float subDeltaTime = deltaTime / substeps;

        for (int i = 0; i < substeps; i++)
        {
            // Apply gravity for freefall if the coin has not yet settled on the plane
            if (!hasCollided || height > planeHeight)
            {
                verticalVelocity -= gravity * subDeltaTime;  // Update vertical velocity
                height += verticalVelocity * subDeltaTime;   // Update height

                // If the coin goes below the plane, set it to the plane height
                if (height <= planeHeight)
                {
                    height = planeHeight; // Prevent the coin from going below the plane
                    if (verticalVelocity < -0.1f)  // Only bounce if falling fast enough
                    {
                        verticalVelocity = -verticalVelocity * bounceDamping; // Reverse and dampen velocity
                    }
                    else
                    {
                        verticalVelocity = 0; // If it's slow, just stop the motion
                    }

                    // Reduce angular velocity to simulate energy loss from impact
                    angularVelocity *= bounceDamping;

                    // Allow tilting after collision
                    hasCollided = true;
                }
            }
            else
            {
                // Apply rolling friction to gradually slow down the angular velocity
                float frictionForce = rollingFriction * mass * gravity;
                float frictionTorque = frictionForce * subDeltaTime;
                angularVelocity -= frictionTorque / mass;
                angularVelocity = Mathf.Max(angularVelocity, 0.0f); // Prevent angular velocity from going negative

                // Gradually increase tilt as the coin settles
                if (tiltAngle < 90.0f)
                {
                    tiltAngle += gravity * subDeltaTime * 0.5f * (1 - tiltDamping);  // Apply tilt with damping
                    tiltAngle = Mathf.Min(tiltAngle, 90.0f); // Cap tilt angle at 90 degrees
                }
            }

            // Update the rotation and tilt of the coin
            currentAngle += angularVelocity * subDeltaTime;
            Quaternion rotation = Quaternion.Euler(tiltAngle, 0, currentAngle);
            transform.rotation = rotation;

            // Manually set the coin's position to avoid passing through the plane
            transform.position = new Vector3(transform.position.x, Mathf.Max(height, planeHeight), transform.position.z);
        }
    }
}