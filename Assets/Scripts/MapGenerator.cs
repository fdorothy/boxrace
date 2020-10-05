using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    Transform[] pieces;
    public Transform player;
    public Landing landingPrefab;
    public Stairs stairsPrefab;
    public Goal goalPrefab;
    public Pickup pickupPrefab;
    public Transform wallPrefab;

    public int width = 10;
    public int height = 10;
    public int depth = 10;
    private static readonly Random random = new Random();

    public Vector3Int startNode;

    public class MapNode
    {
        public bool goal = false;
        public int[] stairs = new int[4];
        public bool[] walls = new bool[4];
    }
    protected MapNode[] nodes;

    protected float spacing = 30.0f;
    protected float levelSpacing = 6.0f;
    protected float stairWidth = 2.0f;

    protected Vector3Int[] dirs = new Vector3Int[4] {
        new Vector3Int(1, 0, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(0, -1, 0)
    };

    public float LevelSpacing { get => levelSpacing; }

    public void PopulateMap()
    {
        Game.singleton.totalCoins = 0;
        Game.singleton.coins = 0;
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        nodes = new MapNode[width * height * depth];
        startNode = new Vector3Int(2, 2, 0);
        RandomTraceDown(startNode, -1);
        CreateMapTiles();
    }

    void RandomTraceDown(Vector3Int v, int incomingDir)
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
                if (incomingDir != i && InBounds(dirs[i] + v + new Vector3Int(0, 0, 1)))
                    possibleDirs.Add(i);
            }

            // only go down if there is at least one possible direction to take
            if (possibleDirs.Count > 0)
            {
                int index = RandomValue(possibleDirs);
                node.stairs[index] = 1;
                for (int i = 0; i < 4; i++)
                {
                    if (index != i && incomingDir != i)
                        node.walls[i] = true;
                    else
                        node.walls[i] = false;
                }
                RandomTraceDown(v + dirs[index] + new Vector3Int(0, 0, 1), Opposite(index));
            }
            else
            {
                node.goal = true;
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
                        if (node.goal)
                            CreateGoal(new Vector3Int(i, j, k));
                        else
                            CreateLanding(new Vector3Int(i, j, k));
                        Vector3Int start = new Vector3Int(i, j, k);
                        Vector3Int end = Vector3Int.zero;
                        bool hasStairs = false;
                        if (node.stairs[0] == 1)
                        {
                            end = new Vector3Int(i + 1, j, k + 1);
                            hasStairs = true;
                        }
                        if (node.stairs[1] == 1)
                        {
                            end = new Vector3Int(i, j + 1, k + 1);
                            hasStairs = true;
                        }
                        if (node.stairs[2] == 1)
                        {
                            end = new Vector3Int(i - 1, j, k + 1);
                            hasStairs = true;
                        }
                        if (node.stairs[3] == 1)
                        {
                            end = new Vector3Int(i, j - 1, k + 1);
                            hasStairs = true;
                        }
                        if (hasStairs)
                        {
                            CreateStairs(start, end);
                            CreatePickups(start, end);
                        }
                        for (int x = 0; x < 4; x++)
                            if (node.walls[x])
                                CreateWallAtEdge(MapToWorld(new Vector3Int(i, j, k)), x);
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

    void CreateGoal(Vector3Int src)
    {
        CreateGoal(MapToWorld(src));
    }

    void CreateGoal(Vector3 center)
    {
        Goal goal = Instantiate(goalPrefab, transform);
        goal.Build(center, stairWidth);
    }

    void CreateStairs(Vector3Int src, Vector3Int dst)
    {
        Vector3Int diff = dst - src;
        Vector3 spacing = (new Vector3(diff.x, 0.0f, diff.y)).normalized * stairWidth / 2.0f;
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

    void CreatePickups(Vector3Int src, Vector3Int dst)
    {
        Vector3Int diff = dst - src;
        Vector3 spacing = (new Vector3(diff.x, 0.0f, diff.y)).normalized * stairWidth / 2.0f;
        CreatePickups(MapToWorld(src) + spacing, MapToWorld(dst) - spacing);
    }

    public void CreatePickups(Vector3 src, Vector3 dst)
    {
        Vector3 v = dst - src;
        const float pickupPerUnit = 1.0f;
        int n = (int)(v.magnitude / pickupPerUnit);
        float dv = 1.0f / (n - 1);
        Vector3 dir = Vector3.Cross(v, Vector3.up).normalized;
        Vector3 perp = dir * stairWidth;
        Vector3 offset = RandomItemOffset(perp);
        int coinsToDrop = Random.Range(3, 10);
        int cooldown = 3;
        for (int i = 0; i < n; i++)
        {
            Vector3 pos = v * i * dv + src + Vector3.up * 0.2f + offset * 0.8f;
            if (cooldown > 0)
            {
                cooldown--;
            }
            else
            {
                if (coinsToDrop > 0)
                {
                    coinsToDrop--;

                    Pickup p = Instantiate<Pickup>(pickupPrefab, transform);
                    p.transform.position = pos;
                    Game.singleton.totalCoins += 1;
                    if (coinsToDrop == 0)
                        cooldown = 2;
                }
                else
                {
                    // see if we should put down a wall here
                    if (Random.Range(0, 3) == 0)
                    {
                        CreateWall(pos);
                    }
                    cooldown = 3;
                    coinsToDrop = Random.Range(3, 10);
                    offset = RandomItemOffset(perp);
                }
            }
        }
    }

    public void CreateWalls(Vector3Int src, Vector3Int dst)
    {
        Vector3Int diff = dst - src;
        Vector3 spacing = (new Vector3(diff.x, 0.0f, diff.y)).normalized * stairWidth / 2.0f;
        CreateWalls(MapToWorld(src) + spacing, MapToWorld(dst) - spacing);
    }

    public void CreateWallAtEdge(Vector3 src, int dir)
    {
        Vector3Int diff = dirs[dir];
        Vector3 spacing = (new Vector3(diff.x, 0.0f, diff.y)).normalized * stairWidth / 2.0f;
        CreateWall(src + spacing, dir % 2 == 0 ? 0.0f : 90.0f);
    }

    public void CreateWalls(Vector3 src, Vector3 dst)
    {
        Vector3 v = dst - src;
        const float wallsPerUnit = 0.2f;
        int n = (int)(v.magnitude * wallsPerUnit);
        float dv = 1.0f / (n - 1);
        Vector3 dir = Vector3.Cross(v, Vector3.up).normalized;
        for (int i = 1; i < n - 1; i++)
        {
            Vector3 pos = v * i * dv + src + Vector3.up * 0.1f;
            CreateWall(pos, dir * stairWidth);
        }
    }

    public void CreateWall(Vector3 src, Vector3 perp)
    {
        Transform p = Instantiate<Transform>(wallPrefab, transform);
        p.position = src + RandomItemOffset(perp);

        // randomize the direction
        p.rotation = Quaternion.AngleAxis((int)(Random.value * 4) * 90.0f + 45.0f, Vector3.up);
    }

    public void CreateWall(Vector3 src)
    {
        CreateWall(src, (int)(Random.value * 4) * 90.0f + 45.0f);
    }

    public void CreateWall(Vector3 src, float angle)
    {
        Transform p = Instantiate<Transform>(wallPrefab, transform);
        p.position = src;

        // randomize the direction
        p.rotation = Quaternion.AngleAxis(angle, Vector3.up);
    }

    public Vector3 RandomItemOffset(Vector3 perp)
    {
        return ((int)(Random.value * 3) / 2.0f - 0.5f) * perp * 0.8f;
    }
}
