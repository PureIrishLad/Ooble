using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OobleAI : MonoBehaviour
{
    public GameObject player;
    public float runProximity;
    public float runDistance;
    public float rotateSpeed;
    public float maxSpeed;

    private float velocity = 0;
    public float acceleration;

    private bool running;

    public float knockoutSpeed = 2f;
    private bool knockedOut = false;

    public Material red;
    private Rigidbody rb;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!knockedOut)
        {
            if (Vector3.Distance(transform.position, player.transform.position) <= runProximity || Vector3.Distance(transform.position, player.transform.position) <= runProximity + runDistance && running)
            {
                running = true;
                Vector3 targetDir = transform.position - player.transform.position;
                targetDir.y = 0;
                float singleStep = rotateSpeed * Time.deltaTime;

                Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDir, singleStep, 0.0f);
                Debug.DrawRay(transform.position, newDirection, Color.red);
                transform.rotation = Quaternion.LookRotation(newDirection);

                velocity += acceleration * Time.deltaTime;
                velocity = Mathf.Min(velocity, maxSpeed);

                transform.position += transform.forward * velocity * Time.deltaTime;

                rb.angularVelocity = Vector3.zero;
            }
            else
            {
                velocity = 0;
                running = false;
                rb.angularVelocity = Vector3.zero;
            }
        }
        else
        {
            velocity = 0;
            running = false;

            GetComponent<Renderer>().material = red;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.rigidbody != null)
            if (collision.rigidbody.tag == "Bat" || collision.rigidbody.tag == "Broom")
                if (collision.rigidbody.velocity.magnitude >= knockoutSpeed)
                {
                    GetComponent<OVRGrabbable>().enabled = true;
                    knockedOut = true;
                }
    }
}
