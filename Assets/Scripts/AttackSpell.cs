using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSpell : MonoBehaviour
{
    public Rigidbody rb;
    public float gravity;
    public float force;

    private GameManager gameManager;

    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        AudioSystemHandler a = Instantiate(gameManager.attackSpellFireAudio, transform.position, Quaternion.identity).GetComponent<AudioSystemHandler>();
        a.Play();
    }

    private void FixedUpdate()
    {
        rb.AddForce(Vector3.down * gravity);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Wand" || other.tag == "Player") return;

        Collider[] colls = Physics.OverlapSphere(transform.position, 50);

        foreach(Collider coll in colls)
        {
            GameObject obj = coll.gameObject;
            Rigidbody objrb = obj.GetComponent<Rigidbody>();

            if (objrb)
            {
                Vector3 displacement = objrb.position - transform.position;
                float magnitude = displacement.magnitude;
                Vector3 direction = displacement / magnitude;

                float f = force / (magnitude * magnitude);
                f = Mathf.Min(force, f);

                if (coll.tag == "Ooble")
                {
                    OobleAI ai = objrb.GetComponent<OobleAI>();
                    if (magnitude <= 2f && ai.running)
                    {
                        ai.knockedOut = true;
                        obj.GetComponent<OVRGrabbable>().enabled = true;
                    }
                    else
                        continue;
                }

                objrb.AddForce(direction * f);
            }
        }

        AudioSystemHandler a = Instantiate(gameManager.attackSpellHitAudio, transform.position, Quaternion.identity).GetComponent<AudioSystemHandler>();
        a.Play();

        Destroy(this.gameObject);
    }
}
