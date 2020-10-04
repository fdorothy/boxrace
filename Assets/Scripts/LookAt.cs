using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    public Transform target;
    protected Vector3 moveTo;

    private void Start()
    {
        target = FindObjectOfType<Player>().transform;
        UpdatePosition();
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(target, Vector3.up);
        transform.position = Vector3.Lerp(transform.position, moveTo+target.position, Time.deltaTime);
    }

    void UpdatePosition()
    {
        float theta = Random.Range(-20.0f, 20.0f) * Mathf.Deg2Rad;
        float gamma = 5.0f * Mathf.Deg2Rad;

        float distance = 10.0f;
        Vector3 u = target.forward * distance;
        u = new Vector3(u.x, 0.0f, u.z);
        Vector3 v = target.right * distance;
        v = new Vector3(v.x, 0.0f, v.z);
        Vector3 w = Vector3.up;

        moveTo = Mathf.Cos(theta) * Mathf.Cos(gamma) * u + Mathf.Sin(theta) * Mathf.Cos(gamma) * v + Mathf.Sin(gamma) * w;

        Invoke("UpdatePosition", 5.0f);
    }
}
