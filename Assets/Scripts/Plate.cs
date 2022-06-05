using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Used for cup and plate shattering behaviour
public class Plate : MonoBehaviour
{
    private Rigidbody rb;
    private GameManager gameManager;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.rigidbody != null && collision.rigidbody.velocity.magnitude > 1 && !collision.rigidbody.isKinematic || rb.velocity.magnitude > 1)
            DestroyThis();
    }

    public void DestroyThis()
    {
        ParticleSystemHandler ps = Instantiate(gameManager.plateSmashParticles, transform.position, Quaternion.identity).GetComponent<ParticleSystemHandler>();
        ps.Play();

        AudioSystemHandler a = Instantiate(gameManager.plateSmashAudio, transform.position, Quaternion.identity).GetComponent<AudioSystemHandler>();
        a.Play();

        Destroy(GetComponent<OVRGrabbable>());
        Destroy(GetComponent<Collider>());
        Destroy(gameObject);
    }
}
