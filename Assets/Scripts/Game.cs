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
        Vector3 startPosition = mapGenerator.MapToWorld(mapGenerator.startNode) + Vector3.up;
        player.Reset(startPosition);
    }

    private void Update()
    {
        if (PlayerFell())
        {
            NewLevel();
        }
    }

    bool PlayerFell()
    {
        return (player.transform.position.y < -mapGenerator.depth * mapGenerator.LevelSpacing);
    }

    public void Win()
    {
        NewLevel();
    }
}
