using UnityEngine;

public class Draggable : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 offset;
    private Camera cam;

    public RigidBodyClone associatedParticle; // This will link the particle to the sphere for position updates
    private Vector3 dragPlaneNormal = Vector3.up; // The normal of the plane on which we will drag (use the world's up direction, or modify if needed)
    private float distanceToPlane; // The distance from the camera to the drag plane
    private float initialZ; // Store the initial Z position for locking

    void Start()
    {
        cam = Camera.main; // Cache the main camera
        initialZ = transform.position.z; // Store the initial Z position
    }

    void Update()
    {
        if (isDragging)
        {
            // Cast a ray from the camera to the mouse position
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Perform the raycast to get the point where the ray intersects with the plane
            if (Physics.Raycast(ray, out hit))
            {
                // Calculate the new position of the sphere based on the raycast hit, keeping the initial Z value
                Vector3 newPos = hit.point + offset;
                newPos.z = initialZ; // Lock the Z-coordinate

                transform.position = newPos; // Update the sphere's position
                associatedParticle.Position = transform.position; // Update the associated particle position
            }
        }
    }

    void OnMouseDown()
    {
        if (associatedParticle != null && associatedParticle.Mass > 0)
        {
            isDragging = true;

            // Cast a ray to the plane and get the distance to the drag plane
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                distanceToPlane = Vector3.Dot(hit.point - cam.transform.position, dragPlaneNormal);
            }

            offset = transform.position - hit.point; // Calculate offset relative to the plane intersection

            // Stop current velocity during drag (already added)
            associatedParticle.Velocity = Vector3.zero;
            associatedParticle.IsBeingDragged = true;
        }
        else
        {
            Debug.LogWarning("Attempted to drag a sphere with no associated particle.");
        }
    }

    void OnMouseUp()
    {
        if (associatedParticle != null)
        {
            associatedParticle.IsBeingDragged = false; // Reset flag when dragging stops
        }
        isDragging = false;

        // Ensure gravity or forces are properly applied again
        Vector3 gravity = new Vector3(0, -9.8f, 0);  // Assuming gravity in y-direction
        associatedParticle.Velocity += gravity * Time.deltaTime;  // Reapply gravity after dragging
    }
}