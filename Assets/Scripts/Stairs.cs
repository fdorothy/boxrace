using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stairs : MonoBehaviour
{
    public void Build(Vector3 src, Vector3 dst, float width)
    {
        BuildCollider(src, dst, width);

        float trimWidth = 0.1f;
        Vector3 diff = dst - src;
        Vector3 dir = diff.normalized;
        Vector3 norm = Vector3.Cross(dir, Vector3.up).normalized * (width - trimWidth) / 2.0f;

        MeshBuilder meshBuilder = new MeshBuilder();
        float stairsPerUnit = 2.0f;
        int stairs = (int)(diff.magnitude * stairsPerUnit);
        Vector3 depth = Vector3.Cross(Vector3.up, norm.normalized) / stairsPerUnit;
        Vector3 height = Vector3.up * (1.0f / (stairs) * (diff.y));
        for (int i=0; i<stairs; i++)
        {
            float t = (float)(i+1) / (stairs);
            AddStair(meshBuilder, diff * t + src, norm, depth, height);
        }
        
        Vector3 trimOffset = norm;
        AddStairTrim(meshBuilder, src - trimOffset, dst - trimOffset, trimWidth, 1.0f);
        AddStairTrim(meshBuilder, src + trimOffset, dst + trimOffset, trimWidth, 1.0f);

        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = meshBuilder.Build();
    }

    void AddStair(MeshBuilder meshBuilder, Vector3 center, Vector3 norm, Vector3 depth, Vector3 height)
    {
        meshBuilder.AddQuad(center - norm - depth/2.0f, center + norm - depth / 2.0f, -depth/2.0f);
        meshBuilder.AddQuad(center - norm + height/2.0f, center + norm + height/2.0f, -height/2.0f);
    }

    void AddStairTrim(MeshBuilder meshBuilder, Vector3 src, Vector3 dst, float width, float depth)
    {
        Vector3 u = dst - src;
        Vector3 v = Vector3.Cross(u.normalized, Vector3.up) * width / 2.0f;
        Vector3 w = Vector3.down * depth;

        meshBuilder.AddQuad(src, dst, v);
        meshBuilder.AddQuad(src+w, dst+w, -v);
        meshBuilder.AddQuad(src + w/2.0f+v, dst + w/2.0f+v, w/2.0f);
        meshBuilder.AddQuad(src + w / 2.0f - v, dst + w / 2.0f - v, -w / 2.0f);
    }

    public void BuildCollider(Vector3 src, Vector3 dst, float width)
    {   
        Mesh mesh = new Mesh();

        Vector3 dir = (dst - src).normalized;
        Vector3 norm = Vector3.Cross(dir, Vector3.up).normalized * width / 2.0f;

        Vector3[] vertices = new Vector3[4]
        {
            src+norm,
            src-norm,
            dst+norm,
            dst-norm
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

        MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
    }
}
