using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSpell : MonoBehaviour
{
    public Rigidbody rb;
    public float gravity;
    public float force;

    private GameManager gameManager;

    private ParticleSystemHandler psTrail;
    private ParticleSystemHandler psExplosion;

    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        AudioSystemHandler a = Instantiate(gameManager.attackSpellFireAudio, transform.position, Quaternion.identity).GetComponent<AudioSystemHandler>();
        psExplosion = Instantiate(gameManager.attackExplosionParticles, transform.position, Quaternion.identity).GetComponent<ParticleSystemHandler>();
        psTrail = Instantiate(gameManager.attackTrailParticles, transform.position, Quaternion.identity).GetComponent<ParticleSystemHandler>();
        a.Play();
        psTrail.Play();
    }

    private void FixedUpdate()
    {
        rb.AddForce(Vector3.down * gravity);
        psTrail.transform.position = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Wand" || other.tag == "Player") return;

        Collider[] colls = Physics.OverlapSphere(transform.position, 50);
        psExplosion.transform.position = transform.position;
        psExplosion.Play();

        ParticleSystem.MainModule ps = psTrail.GetComponent<ParticleSystem>().main;
        ps.loop = false;

        // Applying explosive force to all objects in the vicinity
        foreach(Collider coll in colls)
        {
            if (coll.tag == "Button") continue;
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
                    if (magnitude <= 2f && (ai.running || ai.knockedOut))
                        ai.health -= 25;
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
