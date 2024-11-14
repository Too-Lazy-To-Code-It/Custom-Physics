using UnityEngine;

public class RigidBodyClone
{
    public Vector3 Position;
    public Vector3 PreviousPosition;
    public Vector3 Velocity;
    public Quaternion Orientation; // Rotation
    public Quaternion PreviousOrientation;

    public float Mass;
    public float InverseMass => Mass == 0 ? 0 : 1 / Mass;
    // New flag for manual dragging
    public bool IsBeingDragged;

    public RigidBodyClone(Vector3 position, Quaternion orientation, float mass)
    {
        Position = position;
        PreviousPosition = position;
        Orientation = orientation;
        PreviousOrientation = orientation;
        Mass = mass;
        Velocity = Vector3.zero;
        IsBeingDragged = false; // Default to not being dragged
    }
}

public class DistanceConstraint
{
    public int IndexA;
    public int IndexB;
    public float RestDistance;

    public DistanceConstraint(int indexA, int indexB, float restDistance)
    {
        IndexA = indexA;
        IndexB = indexB;
        RestDistance = restDistance;
    }
}