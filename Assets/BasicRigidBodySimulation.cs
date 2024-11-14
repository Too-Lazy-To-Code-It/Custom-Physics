using UnityEngine;

public class BasicRigidBodySimulation : MonoBehaviour
{
    public GameObject cubeB; // Reference to Cube B
    public Vector3 velocity; // Velocity of Cube A
    public Vector3 position; // Position of Cube A
    public float mass = 1.0f; // Mass of Cube A
    public Vector3 appliedForce; // Force applied to Cube A each frame
    public float gravity = 9.81f; // Gravitational acceleration
    public float bounceFactor = 0.8f; // Bounce factor for ground collision
    public float compliance = 0.001f; // Compliance factor for constraint corrections
    public Vector3 angularVelocity; // Angular velocity of Cube A
    public Vector3 appliedTorque; // Torque applied to Cube A each frame
    public int substeps = 4; // Number of substeps per fixed update

    private Vector3 positionB; // Desired position of Cube B
    public float desiredDistance = 2.0f; // Fixed distance between Cube A and Cube B
    public float followSpeed = 5.0f; // Speed of Cube B's follow response
    public float springDamping = 0.2f; // Damping for smooth spring effect

    private Vector3 cubeBVelocity = Vector3.zero; // Intermediate velocity for Cube B

    void Start()
    {
        position = transform.position; // Initialize Cube A position
        velocity = Vector3.zero;

        if (cubeB != null)
        {
            positionB = cubeB.transform.position; // Initialize Cube B position
        }
    }

    Vector3 CalculateForces()
    {
        Vector3 force = Vector3.zero;
        force += Vector3.down * gravity * mass; // Gravity force
        force += appliedForce; // External force applied to Cube A
        return force;
    }

    void FixedUpdate()
    {
        // Calculate forces and update velocity for Cube A
        Vector3 force = CalculateForces();
        Vector3 acceleration = force / mass;
        velocity += acceleration * Time.fixedDeltaTime;

        // Apply compliance correction for Cube A
        ApplyGroundConstraint(Time.fixedDeltaTime);
        ApplyAngularConstraint(Time.fixedDeltaTime);

        // Apply forces and constraints for Cube A in substeps
        for (int i = 0; i < substeps; i++)
        {
            ApplyForcesAndConstraints(Time.fixedDeltaTime / substeps);
        }

        // Update Cube A's position
        transform.position = position;

        // Smoothly apply distance constraint with a spring effect
        ApplySmoothDistanceConstraint(Time.fixedDeltaTime);

        // Reset applied force for the next frame
        appliedForce = Vector3.zero;
    }

    void ApplyForcesAndConstraints(float deltaTime)
    {
        // Calculate forces for Cube A
        Vector3 force = CalculateForces();
        Vector3 acceleration = force / mass;
        velocity += acceleration * deltaTime;

        // Apply compliance correction for Cube A
        ApplyGroundConstraint(deltaTime);
        ApplyAngularConstraint(deltaTime);

        // Update position based on velocity for Cube A
        position += velocity * deltaTime;

        // Apply rotational dynamics for Cube A
        ApplyTorqueAndRotation(deltaTime);
    }

    void ApplySmoothDistanceConstraint(float deltaTime)
    {
        if (cubeB != null)
        {
            // Calculate target position for Cube B
            Vector3 targetPositionB = position + Vector3.up * desiredDistance;

            // Apply a smooth dampening effect for smooth following
            positionB = Vector3.SmoothDamp(positionB, targetPositionB, ref cubeBVelocity, springDamping, followSpeed, deltaTime);

            // Update Cube B's position
            cubeB.transform.position = positionB;
        }
    }

    void ApplyGroundConstraint(float deltaTime)
    {
        // Example ground constraint (y position >= 0)
        float constraint = position.y - 0;
        if (constraint < 0)
        {
            float lagrangeDelta = compliance * constraint / deltaTime;
            position.y -= lagrangeDelta;
            velocity.y = -velocity.y * bounceFactor;

            if (Mathf.Abs(velocity.y) < 0.1f)
            {
                velocity.y = 0;
            }
        }
    }

    void ApplyAngularConstraint(float deltaTime)
    {
        // Example angular constraint (restrict rotation around the Y-axis)
        Vector3 axis = Vector3.up;
        float angularConstraint = Vector3.Dot(transform.up, axis) - 1.0f;

        if (Mathf.Abs(angularConstraint) > 0.001f)
        {
            float lagrangeDelta = compliance * angularConstraint / deltaTime;
            angularVelocity -= lagrangeDelta * axis;
            angularVelocity *= bounceFactor;
        }
    }

    void ApplyTorqueAndRotation(float deltaTime)
    {
        Vector3 angularAcceleration = appliedTorque / mass;
        angularVelocity += angularAcceleration * deltaTime;

        Quaternion rotation = Quaternion.Euler(angularVelocity * deltaTime);
        transform.rotation = rotation * transform.rotation;
    }

    // Functions to apply force and torque to Cube A
    public void ApplyForce(Vector3 force)
    {
        appliedForce += force;
    }

    public void ApplyTorque(Vector3 torque)
    {
        appliedTorque += torque;
    }

    // Enhanced Gizmo visualization
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + velocity); // Visualize velocity direction
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + appliedForce.normalized); // Visualize force direction
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(position, 0.1f); // Position of Cube A
        if (cubeB != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(positionB, 0.1f); // Position of Cube B
        }
    }
}
