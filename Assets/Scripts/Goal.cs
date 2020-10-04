using UnityEngine;

public class Goal : Landing
{
    private void OnCollisionEnter(Collision collision)
    {
        FindObjectOfType<Game>().Win();
    }
}
