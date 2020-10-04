using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public MapGenerator mapGenerator;
    public Player player;
    public TextManager textManager;

    public int coins = 0;
    public int totalCoins = 0;

    public static Game singleton;

    protected List<string> introText = new List<string>()
    {
        "Why is there so much junk on the stairs, <i>child</i>?",
        "Pick up all the <b>coins</b>!",
        "Don't fall off and break your head open again. Those medical bills are too much.",
        "Remember, I'm <i>always</i> watching you.",
        "Use the <b>arrow keys</b> or <b>wasd</b> to move, and <b>spacebar</b> to jump"
    };

    protected List<string> failureText = new List<string>()
    {
        "Only <b>xxx</b> coins? Pathetic, child. When I was your age I could have at least picked up <b>yyy</b> coins.",
        "Try again, except this time try to pick up coins instead of bringing shame to this family.",
        "I want those stairs clean!"
    };

    protected List<string> successText = new List<string>()
    {
        "What is this? You got all <b>xxx</b> coins?",
        "Always doing the bare minimum, I see. Your brother would have done better.",
        "I guess that's enough for now. I'll let you get back to cooking my dinner.",
        "Game over.\nFollow me on @redmountainman1 or https://fredric.itch.io"
    };

    // Start is called before the first frame update
    void Start()
    {
        singleton = this;
        NewLevel();
        textManager.ShowText(introText, () => { });
    }

    void NewLevel()
    {
        mapGenerator.PopulateMap();
        Vector3 startPosition = mapGenerator.MapToWorld(mapGenerator.startNode) + Vector3.up;
        player.Reset(startPosition);
    }

    private void Update()
    {
        if (PlayerFell() || Input.GetKeyDown(KeyCode.Escape))
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
        if (coins == totalCoins)
        {
            List<string> text = new List<string>();
            for (int i = 0; i < successText.Count; i++)
            {
                text.Add(successText[i].Replace("xxx", coins.ToString()).Replace("yyy", totalCoins.ToString()));
            }
            textManager.ShowText(text, () => { });
        } else
        {
            List<string> text = new List<string>();
            for (int i=0; i<failureText.Count; i++)
            {
                text.Add(failureText[i].Replace("xxx", coins.ToString()).Replace("yyy", totalCoins.ToString()));
            }
            textManager.ShowText(text, () => { });
        }
        NewLevel();
    }
}
