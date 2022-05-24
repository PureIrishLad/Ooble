using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSpell : MonoBehaviour
{
    public Rigidbody rb;
    public float gravity;
    public float force;

    private void FixedUpdate()
    {
        rb.AddForce(Vector3.down * gravity);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Wand" || other.tag == "Player") return;

        GameManager manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        foreach(GameObject ooble in manager.oobles)
        {
            Rigidbody oobleRB = ooble.GetComponent<Rigidbody>();
            Vector3 displacement = ooble.transform.position - transform.position;
            float magnitude = displacement.magnitude;
            Vector3 direction = displacement / magnitude;

            float f = force / (magnitude * magnitude);
            f = Mathf.Min(force, f);

            oobleRB.AddForce(direction * f);

            if (magnitude <= 2f)
                ooble.GetComponent<OobleAI>().knockedOut = true;
        }

        Destroy(this.gameObject);
    }
}
