﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    Transform[] pieces;
    public Transform player;
    public Landing landingPrefab;
    public Stairs stairsPrefab;

    public int width = 10;
    public int height = 10;
    public int depth = 10;
    private static readonly Random random = new Random();

    public Vector3Int startNode;

    public class MapNode
    {
        public int[] stairs = new int[4];
    }
    protected MapNode[] nodes;

    protected float spacing = 5.0f;
    protected float levelSpacing = 2.0f;
    protected float stairWidth = 1.0f;

    protected Vector3Int[] dirs = new Vector3Int[4] {
        new Vector3Int(1, 0, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(0, -1, 0)
    };

    public void PopulateMap()
    {
        nodes = new MapNode[width * height * depth];
        startNode = new Vector3Int(2, 2, 0);
        RandomTraceDown(startNode);
        CreateMapTiles();
    }

    void RandomTraceDown(Vector3Int v)
    {
        if (GetNode(v) == null)
        {
            // add the landing pad
            MapNode node = new MapNode();
            SetNode(v, node);

            // figure out where we can go down from here
            HashSet<int> possibleDirs = new HashSet<int>();
            for (int i = 0; i < dirs.Length; i++)
            {
                // only include if in the bounds of the game
                if (InBounds(dirs[i] + v + new Vector3Int(0, 0, 1)))
                    possibleDirs.Add(i);
            }

            // only go down if there is at least one possible direction to take
            if (possibleDirs.Count > 0)
            {
                int index = RandomValue(possibleDirs);
                node.stairs[index] = 1;
                RandomTraceDown(v + dirs[index] + new Vector3Int(0, 0, 1));
            }
        }
    }

    public T RandomValue<T>(HashSet<T> bag) => bag.ElementAt(Random.Range(0, bag.Count));

    public int Opposite(int dir) => (dir + 2) % 4;

    public bool InBounds(Vector3Int p) => p.x >= 0 && p.y >= 0 && p.z >= 0 && p.x < width && p.y < height && p.z < depth;

    public MapNode GetNode(Vector3Int p)
    {
        if (InBounds(p))
            return nodes[p.x + p.y * width + p.z * height * width];
        return null;
    }
    public void SetNode(Vector3Int p, MapNode node)
    {
        if (InBounds(p))
            nodes[p.x + p.y * width + p.z * height * width] = node;
    }

    void CreateMapTiles()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                for (int k = 0; k < depth; k++)
                {
                    MapNode node = nodes[i + j * width + k * width * height];
                    if (node != null)
                    {
                        CreateLanding(new Vector3Int(i, j, k));
                        if (node.stairs[0] == 1)
                            CreateStairs(new Vector3Int(i, j, k), new Vector3Int(i + 1, j, k + 1));
                        if (node.stairs[1] == 1)
                            CreateStairs(new Vector3Int(i, j, k), new Vector3Int(i, j + 1, k + 1));
                        if (node.stairs[2] == 1)
                            CreateStairs(new Vector3Int(i, j, k), new Vector3Int(i - 1, j, k + 1));
                        if (node.stairs[3] == 1)
                            CreateStairs(new Vector3Int(i, j, k), new Vector3Int(i, j - 1, k + 1));
                    }
                }
            }
        }
    }

    void CreateLanding(Vector3Int src)
    {
        CreateLanding(MapToWorld(src));
    }

    void CreateLanding(Vector3 center)
    {
        Landing landing = Instantiate(landingPrefab, transform);
        landing.Build(center, stairWidth);
    }

    void CreateStairs(Vector3Int src, Vector3Int dst)
    {
        Vector3Int diff = dst - src;
        Vector3 spacing = (new Vector3(diff.x, 0.0f, diff.y)).normalized * stairWidth / 2.0f;
        Debug.Log("spacing = " + spacing);
        CreateStairs(MapToWorld(src) + spacing, MapToWorld(dst) - spacing);
    }

    void CreateStairs(Vector3 src, Vector3 dst)
    {
        Stairs stairs = Instantiate(stairsPrefab, transform);
        stairs.Build(src, dst, stairWidth);
    }

    public Vector3 MapToWorld(Vector3Int v)
    {
        return new Vector3(v.x * spacing, -v.z * levelSpacing, v.y * spacing);
    }
}