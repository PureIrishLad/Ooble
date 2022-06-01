using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemHandler : MonoBehaviour
{
    private ParticleSystem ps;
    [HideInInspector]
    public bool isPlaying;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (isPlaying && ps.isStopped)
            Destroy(gameObject);
    }

    public void Play()
    {
        ps.Play();
        isPlaying = true;
    }
}
