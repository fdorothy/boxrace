using System.Collections.Generic;
using UnityEngine;

class MeshBuilder
{
    List<Vector3> vertices = new List<Vector3>();
    List<Vector2> uv = new List<Vector2>();

    public void AddTriangle(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        vertices.Add(p1);
        vertices.Add(p2);
        vertices.Add(p3);
    }

    public void AddUV(Vector2 uv)
    {
        this.uv.Add(uv);
    }

    public Mesh Build()
    {
        Vector3[] v = vertices.ToArray();
        int[] t = new int[v.Length];
        Vector2[] tex = uv.ToArray();
        Vector3[] norms = new Vector3[v.Length];
        for (int i = 0; i < v.Length; i+=3)
        {
            Vector3 p1 = v[i];
            Vector3 p2 = v[i + 1];
            Vector3 p3 = v[i + 2];
            Vector3 n = Vector3.Cross((p2 - p1).normalized, (p3 - p1).normalized);
            t[i] = i;
            t[i + 1] = i+1;
            t[i + 2] = i + 2;
            norms[i] = n;
            norms[i+1] = n;
            norms[i+2] = n;
        }

        Mesh mesh = new Mesh();
        mesh.vertices = v;
        mesh.normals = norms;
        mesh.triangles = t;
        mesh.uv = tex;
        return mesh;
    }

    public MeshBuilder Combine(MeshBuilder other)
    {
        MeshBuilder m = new MeshBuilder();
        m.vertices.AddRange(vertices);
        m.vertices.AddRange(other.vertices);
        m.uv.AddRange(uv);
        m.uv.AddRange(other.uv);
        return m;
    }

    public void AddQuad(Vector3 src, Vector3 dst, Vector3 u)
    {
        Vector3 a = (dst - src).normalized;
        Vector3 b = u.normalized;

        Vector3 p1 = src - u;
        Vector3 p2 = src + u;
        Vector3 p3 = dst + u;
        AddTriangle(p1, p2, p3);
        AddUV(new Vector2(Vector3.Dot(p1, a), Vector3.Dot(p1, b)));
        AddUV(new Vector2(Vector3.Dot(p2, a), Vector3.Dot(p2, b)));
        AddUV(new Vector2(Vector3.Dot(p3, a), Vector3.Dot(p3, b)));

        p1 = src - u;
        p2 = dst + u;
        p3 = dst - u;
        AddTriangle(p1, p2, p3);
        AddUV(new Vector2(Vector3.Dot(p1, a), Vector3.Dot(p1, b)));
        AddUV(new Vector2(Vector3.Dot(p2, a), Vector3.Dot(p2, b)));
        AddUV(new Vector2(Vector3.Dot(p3, a), Vector3.Dot(p3, b)));
    }
}
