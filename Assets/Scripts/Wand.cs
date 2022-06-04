using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Wand : MonoBehaviour
{
    public GameObject leftHand;
    public GameObject rightHand;

    public GameObject attackSpell;
    public float telekenisisStrength = 0.05f;

    private GameManager gameManager;
    private GameObject[] oobles;
    private bool isActiveR = false;
    private bool wasActiveR;
    private bool isActiveL = false;
    private bool wasActiveL;

    public float attackCooldown;
    private float cooldownTimer;
    private bool cooldown = false;

    private AudioSystemHandler aTelekinesis;

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        oobles = gameManager.oobles;
    }

    private void Update()
    {
        if (cooldown)
        {
            cooldownTimer += Time.deltaTime;

            if (cooldownTimer > attackCooldown)
            {
                cooldownTimer = 0;
                cooldown = false;
            }
        }

        isActiveR = false;
        isActiveL = false;

        if (gameManager.rightController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondaryValueR) && secondaryValueR)
            isActiveR = true;
        if (!cooldown && gameManager.rightController.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryValueR) && primaryValueR)
        {
            Rigidbody rb = Instantiate(attackSpell, rightHand.transform.position + rightHand.transform.forward * 0.21f, Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(rightHand.transform.forward * 500);
            cooldown = true;
        }

        if (gameManager.leftController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondaryValueL) && secondaryValueL)
            isActiveL = true;
        if (!cooldown && gameManager.leftController.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryValueL) && primaryValueL)
        {
            Rigidbody rb = Instantiate(attackSpell, leftHand.transform.position + leftHand.transform.forward * 0.21f, Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(leftHand.transform.forward * 500);
            cooldown = true;
        }

        if (isActiveR || isActiveL)
        {
            foreach(GameObject ooble in oobles)
            {
                if (ooble && ooble.GetComponent<OobleAI>().running || ooble && ooble.GetComponent<OobleAI>().knockedOut)
                {
                    Rigidbody rb = ooble.GetComponent<Rigidbody>();
                    rb.useGravity = true;

                    if (isActiveR)
                    {
                        Vector3 displacement = rightHand.transform.position - ooble.transform.position;
                        float magnitude = displacement.magnitude;
                        Vector3 direction = displacement / magnitude;
                        float force = telekenisisStrength / magnitude;

                        if (magnitude < 3f)
                            rb.useGravity = false;

                        rb.AddForce(direction * force);
                    }

                    if (isActiveL)
                    {
                        Vector3 displacement = leftHand.transform.position - ooble.transform.position;
                        float magnitude = displacement.magnitude;
                        Vector3 direction = displacement / magnitude;
                        float force = telekenisisStrength / magnitude;

                        if (magnitude < 3f)
                            rb.useGravity = false;

                        rb.AddForce(direction * force);
                    }
                }
            }


            if (!aTelekinesis)
            {
                aTelekinesis = Instantiate(gameManager.wandTelekinesisAudio, transform.position, Quaternion.identity).GetComponent<AudioSystemHandler>();
                aTelekinesis.Play();
                aTelekinesis.target = transform;
            }
        }
        else
        {
            if (aTelekinesis)
                Destroy(aTelekinesis.gameObject);

            foreach (GameObject ooble in gameManager.oobles)
                ooble.GetComponent<Rigidbody>().useGravity = true;
        }

        if (isActiveR != wasActiveR && wasActiveR && isActiveL != wasActiveL && wasActiveL)
            foreach (GameObject ooble in oobles)
                ooble.GetComponent<Rigidbody>().useGravity = true;

        wasActiveR = isActiveR;
        wasActiveL = isActiveL;
    }
}
