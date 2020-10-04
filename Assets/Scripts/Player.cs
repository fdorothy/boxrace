using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody rb;
    public float jumpForce = 2.0f;
    float jumpCooldown = 0.0f;
    public float hopForwardForce = 0.1f;
    public float leanForce = 0.1f;
    public float rotationSpeed = 180.0f;
    public float breakSpeed = 0.5f;
    public LayerMask terrainMask;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float dx = Input.GetAxis("Horizontal");
        float dy = Input.GetAxis("Vertical");
        bool doJump = (dy > 0.01f || Mathf.Abs(dx) > 0.01f) && rb.velocity.magnitude <= 0.1f;
        if (jumpCooldown <= 0.0f && (Input.GetKeyDown(KeyCode.Space) || doJump))
        {
            // jump
            if (OnGround())
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                jumpCooldown = 0.25f;
            }
        }

        if (jumpCooldown > 0.0f)
            jumpCooldown -= Time.deltaTime;

        float dt = Time.deltaTime;
        if (!OnGround())
        {
            rb.AddRelativeForce(Vector3.forward * dy * hopForwardForce, ForceMode.Impulse);
            transform.Rotate(Vector3.up, dt * dx * rotationSpeed);
        }
        else
        {
            if (rb.velocity.magnitude > 1.0f)
            {
                Vector3 right = Vector3.Cross(rb.velocity.normalized, transform.up);
                rb.AddForce(-right * dx * leanForce, ForceMode.VelocityChange);
                if (dy < -0.1f && rb.velocity.magnitude > 0.0f)
                {
                    rb.AddForce(-breakSpeed * rb.velocity, ForceMode.Acceleration);
                }

                LookTowards(rb.velocity.normalized, 2f);
            }
        }
    }

    void LookTowards(Vector3 targetDirection, float speed)
    {
        Quaternion dirQ = Quaternion.LookRotation(targetDirection);
        Quaternion slerp = Quaternion.Slerp(transform.rotation, dirQ, targetDirection.magnitude * speed * Time.deltaTime);
        rb.MoveRotation(slerp);
    }

    bool OnGround()
    {
        RaycastHit hitInfo;
        return Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out hitInfo, 0.15f, terrainMask.value);
    }

    public void Reset(Vector3 startPosition)
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.MoveRotation(Quaternion.identity);
        rb.MovePosition(startPosition);
        rb.isKinematic = true;
        Invoke("Release", 1.0f);
    }

    public void Release()
    {
        rb.isKinematic = false;
    }
}
