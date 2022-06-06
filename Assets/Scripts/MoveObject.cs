using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    public float maxYOffset;
    public float maxSpeed;
    public Vector3 rotationSpeed;

    private Vector3 initialPos;
    private Vector3 maxPos;
    private float t = 0;
    private bool down = false;
    private float speed;

    private void Start()
    {
        initialPos = transform.position;
        maxPos = initialPos + new Vector3(0, maxYOffset, 0);

        speed = maxSpeed;
        t = 0.001f;
    }

    private void Update()
    {
        if (t >= 1)
        {
            t = 1;
            down = true;
        }
        else if (t <= 0)
        {
            t = 0;
            down = false;
        }

        speed = 0.05f + ((t < 0.5f ? t : (1 - t)) / maxSpeed);
        transform.position = Vector3.Lerp(initialPos, maxPos, t);
        t += (down ? -Time.deltaTime : Time.deltaTime) * speed;

        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + (rotationSpeed * Time.deltaTime));
    }
}
