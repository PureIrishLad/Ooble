using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportHandler : MonoBehaviour
{
    private Rigidbody rb;

    private float time = 0.05f;
    private float timer = 0;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void LateUpdate()
    {
        if (rb.collisionDetectionMode == CollisionDetectionMode.Discrete)
            timer += Time.deltaTime;

        if (timer > time)
        {
            timer = 0;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }
    }
}
