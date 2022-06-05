using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script for easy use audio system
public class AudioSystemHandler : MonoBehaviour
{
    private AudioSource a;
    
    [HideInInspector]
    public bool isPlaying;

    public bool destroyOnStop = true;
    public bool followObject = false;

    [HideInInspector]
    public Transform target;

    private void Awake()
    {
        a = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (followObject && target)
            transform.position = target.position;

        if (destroyOnStop && isPlaying && !a.isPlaying)
            Destroy(gameObject);
    }

    public void Play()
    {
        a.Play();
        isPlaying = true;
    }
}
