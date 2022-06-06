using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Wand : MonoBehaviour
{
    public GameObject leftHand; // Left hand object
    public GameObject rightHand; // Right hand object

    public GameObject attackSpell; // Attack spell prefab
    public float telekenisisStrength = 0.05f; // Strength of telekinesis

    private GameManager gameManager; // Reference to game manager
    private GameObject[] oobles; // Array of all oobles

    // Variables for telekinesis being enable for both hands
    private bool isActiveR = false;
    private bool wasActiveR;
    private bool isActiveL = false;
    private bool wasActiveL;

    // Cooldown for shooting
    public float attackCooldown;
    private float cooldownTimer;
    private bool cooldown = false;

    // Reference to the telekinesis audio
    private AudioSystemHandler aTelekinesis;

    // References to the particle systems for the left and right hand telekinesis
    private ParticleSystemHandler pTelekinesisL;
    private ParticleSystemHandler pTelekinesisR;

    private bool lastShotFromRight = false;
    private bool lastShotFromLeft = false;

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

        bool primaryValueR = false;
        bool primaryValueL = false;

        // Getting controller inputs
        if (gameManager.rightController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondaryValueR) && secondaryValueR)
            isActiveR = true;
        if (gameManager.rightController.TryGetFeatureValue(CommonUsages.primaryButton, out primaryValueR) && primaryValueR && !lastShotFromRight && !cooldown)
        {
            Rigidbody rb = Instantiate(attackSpell, rightHand.transform.position + rightHand.transform.forward * 0.21f, Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(rightHand.transform.forward * 500);
            cooldown = true;
            lastShotFromRight = true;
            lastShotFromLeft = false;
        }

        if (gameManager.leftController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondaryValueL) && secondaryValueL)
            isActiveL = true;
        if (gameManager.leftController.TryGetFeatureValue(CommonUsages.primaryButton, out primaryValueL) && primaryValueL && !lastShotFromLeft && !cooldown)
        {
            Rigidbody rb = Instantiate(attackSpell, leftHand.transform.position + leftHand.transform.forward * 0.21f, Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(leftHand.transform.forward * 500);
            cooldown = true;
            lastShotFromRight = false;
            lastShotFromLeft = true;
        }

        if (!(primaryValueL && primaryValueR))
        {
            lastShotFromLeft = false;
            lastShotFromRight = false;
        }

        if (isActiveR || isActiveL)
        {
            if (isActiveR)
            {
                if (!pTelekinesisR) 
                {
                    pTelekinesisR = Instantiate(gameManager.telekinesisParticles).GetComponent<ParticleSystemHandler>();
                    pTelekinesisR.Play();
                }

                pTelekinesisR.transform.position = rightHand.transform.position;
            }

            if (isActiveL)
            {
                if (!pTelekinesisL)
                {
                    pTelekinesisL = Instantiate(gameManager.telekinesisParticles).GetComponent<ParticleSystemHandler>();
                    pTelekinesisL.Play();
                }

                pTelekinesisL.transform.position = leftHand.transform.position;
            }

            // Applying telekinesis forces to all oobles
            foreach (GameObject ooble in oobles)
            {
                if (ooble && (ooble.GetComponent<OobleAI>().running || ooble.GetComponent<OobleAI>().knockedOut))
                {
                    Rigidbody rb = ooble.GetComponent<Rigidbody>();
                    if (rb == null) continue;

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

            // Creating a new telekinesis audio object
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

        if (!isActiveR && pTelekinesisR)
        {
            pTelekinesisR.Stop();
            pTelekinesisR = null;
        }

        if (!isActiveL && pTelekinesisL)
        {
            pTelekinesisL.Stop();
            pTelekinesisL = null;
        }

        if (isActiveR != wasActiveR && wasActiveR && isActiveL != wasActiveL && wasActiveL)
            foreach (GameObject ooble in oobles)
                ooble.GetComponent<Rigidbody>().useGravity = true;

        wasActiveR = isActiveR;
        wasActiveL = isActiveL;
    }
}
