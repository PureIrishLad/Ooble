using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script for easy to use particle system
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
        if (ps.particleCount == 0 && isPlaying && ps.isStopped)
            Destroy(gameObject);
    }

    public void Play()
    {
        ps.Play();
        isPlaying = true;
    }

    public void Stop()
    {
        ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }
}
