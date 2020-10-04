using UnityEngine;

public class Landing : MonoBehaviour
{
    public void Build(Vector3 center, float width)
    {
        BuildCollider(center, width);

        MeshBuilder meshBuilder = new MeshBuilder();

        float depth = 1f;
        Vector3 u = Vector3.left * width/2.0f;
        Vector3 v = Vector3.forward * width / 2.0f;
        Vector3 w = Vector3.down * depth;

        meshBuilder.AddQuad(-v, v, u);
        meshBuilder.AddQuad(-v+w, v+w, -u);
        meshBuilder.AddQuad(-u-v + w/2.0f, -u+v+w/2.0f, -w/2.0f);
        meshBuilder.AddQuad(u - v + w / 2.0f, u + v + w / 2.0f, w / 2.0f);
        meshBuilder.AddQuad(v - u + w / 2.0f, v + u + w / 2.0f, -w / 2.0f);
        meshBuilder.AddQuad(-v - u + w / 2.0f, -v + u + w / 2.0f, w / 2.0f);

        transform.position = center;
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = meshBuilder.Build();
    }

    public void BuildCollider(Vector3 center, float width)
    {
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

        MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
    }
}
