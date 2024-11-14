using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class CustomCube : MonoBehaviour
{
    public Mesh mesh;

    void Start()
    {
        // Check if the MeshFilter component is attached; if not, add one
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }

        // Check if the MeshRenderer component is attached; if not, add one
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
        }

        // Define the vertices of the cube
        Vector3[] vertices = new Vector3[]
        {
            new Vector3(-1, -1, -1), // back-bottom-left
            new Vector3( 1, -1, -1), // back-bottom-right
            new Vector3( 1,  1, -1), // back-top-right
            new Vector3(-1,  1, -1), // back-top-left
            new Vector3(-1, -1,  1), // front-bottom-left
            new Vector3( 1, -1,  1), // front-bottom-right
            new Vector3( 1,  1,  1), // front-top-right
            new Vector3(-1,  1,  1)  // front-top-left
        };

        // Define the triangles
        int[] triangles = new int[]
        {
            // Back face
            0, 2, 1, 0, 3, 2,
            // Front face
            4, 5, 6, 4, 6, 7,
            // Left face
            0, 7, 3, 0, 4, 7,
            // Right face
            1, 2, 6, 1, 6, 5,
            // Bottom face
            0, 1, 5, 0, 5, 4,
            // Top face
            3, 7, 6, 3, 6, 2
        };

        // Create the Mesh
        mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        // Apply the Mesh to the MeshFilter component
        meshFilter.mesh = mesh;
    }

    // Apply a transformation matrix to the cube's vertices
    public void ApplyTransformation(Matrix4x4 matrix)
    {
        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            // Transform each vertex using the matrix
            vertices[i] = matrix.MultiplyPoint3x4(vertices[i]);
        }
        mesh.vertices = vertices;
        mesh.RecalculateBounds();  // Recalculate mesh bounds
    }
}