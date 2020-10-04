using UnityEngine;

public class Pickup : MonoBehaviour
{
    float width;
    float t = 0.0f, t0;
    float speed;
    Vector3 initialPosition;
    Vector3 dir;

    private void Update()
    {
        //transform.position = initialPosition + dir * width * Mathf.Cos(t0 + t*speed) / 2.0f;
        //t += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        Game.singleton.coins += 1;
        Destroy(gameObject);
    }

    public void Wander(float t, float speed, float width, Vector3 pos, Vector3 dir)
    {
        this.t0 = t;
        this.speed = speed;
        this.width = width;
        this.initialPosition = pos;
        this.dir = dir;
        this.transform.position = pos;
    }
}
