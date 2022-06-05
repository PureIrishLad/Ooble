using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlphaLerp : MonoBehaviour
{
    public float alphaStart = 0.0f;
    public float alphaEnd = 1.0f;
    public float duration = 1.0f;
    public float distance = 2.0f;

    public Renderer renderer;

    private Transform player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        renderer.enabled = Vector3.Distance(transform.position, player.position) < distance;

        float lerp = Mathf.PingPong(Time.time, duration) / duration;
        Color c = renderer.material.color;
        c.a = Mathf.Lerp(alphaStart, alphaEnd, lerp);
        renderer.material.color = c;
    }
}
