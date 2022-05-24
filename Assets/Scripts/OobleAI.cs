using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OobleAI : MonoBehaviour
{
    private GameObject player; // The player

    public float knockoutSpeed = 2f; // How fast the player needs to swing to knockout the Ooble
    public bool knockedOut = false; // True if the Ooble is knocked out
    public Material red; // The material applied to the Ooble after being knocked out

    private Renderer oobleRenderer; // This objects renderer

    private GameObject collectionUnit;
    [HideInInspector]
    public Vector3 initialPos;

    [HideInInspector]
    public ParticleSystemHandler ps;

    [HideInInspector]
    public bool destroy;

    private Vector3 initialScale;
    private float lerp;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        oobleRenderer = GetComponent<Renderer>();
        collectionUnit = GameObject.FindGameObjectWithTag("CollectionUnit");

        initialScale = transform.localScale;
    }

    private void Update()
    {
        if (destroy)
            DestroyThis();   
        
        if (knockedOut)
            oobleRenderer.material = red;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.rigidbody != null && collision.rigidbody.velocity.magnitude >= knockoutSpeed && collision.rigidbody.tag == "Bat")
        {
            GetComponent<OVRGrabbable>().enabled = true;
            knockedOut = true;
            oobleRenderer.material = red;
        }
    }
    
    private void DestroyThis()
    {
        transform.localScale = Vector3.Lerp(initialScale, new Vector3(0, 0, 0), lerp);
        transform.position = Vector3.Lerp(initialPos, collectionUnit.transform.position, lerp);
        lerp += Time.deltaTime * 2;

        if (lerp > 1.0f)
        {
            ps.Play();
            Destroy(gameObject);
        }
    }
}
