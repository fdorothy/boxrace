using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public MapGenerator mapGenerator;
    public Player player;

    // Start is called before the first frame update
    void Start()
    {
        NewLevel();
    }

    void NewLevel()
    {
        mapGenerator.PopulateMap();
        player.transform.position = mapGenerator.MapToWorld(mapGenerator.startNode) + Vector3.up;
    }
}
