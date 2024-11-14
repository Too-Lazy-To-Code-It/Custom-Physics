using UnityEngine;

public class ForceApplierOnClick : MonoBehaviour
{
    public float forceMagnitude = 10f; // Force applied on click

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                BasicRigidBodySimulation rbSim = hit.collider.GetComponent<BasicRigidBodySimulation>();

                if (rbSim != null)
                {
                    // Apply force in the direction of the click
                    rbSim.ApplyForce(Vector3.up * forceMagnitude);
                }
            }
        }
    }
}
