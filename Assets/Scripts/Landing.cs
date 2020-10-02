using UnityEngine;

public class Landing : MonoBehaviour
{
    public void Build(Vector3 center, float width)
    {
        transform.position = center;
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));

        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();

        Mesh mesh = new Mesh();

        float w = width / 2.0f;
        Vector3[] vertices = new Vector3[4]
        {
            new Vector3(-w, 0, -w),
            new Vector3(w, 0, -w),
            new Vector3(-w, 0, w),
            new Vector3(w, 0, w)
        };
        mesh.vertices = vertices;

        int[] tris = new int[6]
        {
            // lower left triangle
            0, 2, 1,
            // upper right triangle
            2, 3, 1
        };
        mesh.triangles = tris;

        Vector3 n = Vector3.up;
        mesh.normals = new Vector3[4] { n, n, n, n };

        Vector2[] uv = new Vector2[4]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        };
        mesh.uv = uv;

        meshFilter.mesh = mesh;

        MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
    }
}
