using System.Collections.Generic;
using UnityEngine;

public class XPBD : MonoBehaviour
{
    public int ChainLength = 10; // Number of links in the chain
    public float LinkLength = 1f; // Distance between links
    public float Mass = 1f; // Mass of each link
    public int Iterations = 10; // Number of constraint solver iterations
    public float TimeStep = 0.005f; // Smaller value for more precise simulation

    public List<RigidBodyClone> particles = new List<RigidBodyClone>();
    public List<DistanceConstraint> constraints = new List<DistanceConstraint>();
    private List<GameObject> sphereObjects = new List<GameObject>();

    public GameObject ParticlePrefab; // Assign a Sphere prefab in Unity

    void Start()
    {
        InitializeChain();
        Debug.Log("Chain Initialized: " + particles.Count + " particles, " + constraints.Count + " constraints.");
    }

    void Update()
    {
        Simulate(TimeStep);
        Debug.Log("First Particle Position: " + particles[0].Position + ", Last Particle Position: " + particles[particles.Count - 1].Position);

        // Update the sphere positions based on the particles
        for (int i = 0; i < particles.Count; i++)
        {
            if (sphereObjects[i] != null)
            {
                sphereObjects[i].transform.position = particles[i].Position;
            }
        }
    }


    void InitializeChain()
    {
        Vector3 startPosition = transform.position;

        for (int i = 0; i < ChainLength; i++)
        {
            // Create particles
            particles.Add(new RigidBodyClone(startPosition + i * Vector3.down * LinkLength, Quaternion.identity, Mass));

            // Add a constraint to link the current particle with the previous one
            if (i > 0)
            {
                constraints.Add(new DistanceConstraint(i - 1, i, LinkLength));
            }

            // Optional: Visual debug, create spheres
            if (ParticlePrefab != null)
            {
                var sphere = Instantiate(ParticlePrefab, particles[i].Position, Quaternion.identity, transform);
                sphereObjects.Add(sphere);

                // Attach the Draggable script and link the particle
                Draggable draggable = sphere.AddComponent<Draggable>();
                draggable.associatedParticle = particles[i];
                Debug.Log($"Linked particle {i} with position {particles[i].Position} to sphere.");
            }
        }

        // Fix the first particle (ceiling attachment)
        particles[0].Mass = 0; // Fixed particle (cannot move)
    }

    void Simulate(float dt)
    {
        ApplyExternalForces(Vector3.down * 9.8f, dt);
        VerletIntegration(dt);

        // Solve constraints to maintain chain integrity
        for (int i = 0; i < Iterations; i++)
        {
            SolveConstraints(dt);
        }
        UpdateVelocities(dt);
    }

    void ApplyExternalForces(Vector3 force, float dt)
    {
        foreach (var particle in particles)
        {
            if (particle.Mass > 0)
            {
                // Apply gravity
                particle.Velocity += force * dt;

                // Apply damping to external forces (e.g., gravity)
                particle.Velocity *= 0.98f; // Dampen gravity's effect over time
            }
        }
    }

    void VerletIntegration(float dt)
    {
        foreach (var particle in particles)
        {
            if (particle.Mass > 0)
            {
                Vector3 newPosition = particle.Position + (particle.Position - particle.PreviousPosition) + particle.Velocity * dt * dt;

                // Apply damping to position update
                Vector3 positionDelta = newPosition - particle.Position;
                positionDelta *= 0.98f; // Apply damping to position changes

                particle.PreviousPosition = particle.Position; // Update previous position
                particle.Position += positionDelta; // Update current position
            }
        }
    }



    void SolveConstraints(float dt)
    {
        foreach (var constraint in constraints)
        {
            var pA = particles[constraint.IndexA];
            var pB = particles[constraint.IndexB];

            Vector3 delta = pB.Position - pA.Position;
            float currentDistance = delta.magnitude;

            float compliance = 0.0001f; // Adjust this for stiffer/softer constraints
            float effectiveMass = pA.InverseMass + pB.InverseMass + compliance / (dt * dt);

            if (currentDistance > 0 && effectiveMass > 0) // Prevent division by zero
            {
                float correctionFactor = (currentDistance - constraint.RestDistance) / (currentDistance * effectiveMass);
                Vector3 correction = delta * correctionFactor;

                // Apply a small correction to prevent excessive movement
                correction = Vector3.ClampMagnitude(correction, 0.05f); // Adjust this value as needed

                if (pA.Mass > 0)
                    pA.Position += correction * pA.InverseMass;
                if (pB.Mass > 0)
                    pB.Position -= correction * pB.InverseMass;
            }
        }
    }


    void UpdateVelocities(float dt)
    {
        foreach (var particle in particles)
        {
            if (particle.Mass > 0)
            {
                // Calculate velocity
                particle.Velocity = (particle.Position - particle.PreviousPosition) / dt;
            }

            // Apply damping to the velocity (this will reduce the speed of movement)
            particle.Velocity *= 0.9f; // Damping factor (0.98f means the velocity is reduced by 2% each frame)
        }
    }
}