using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Wand : MonoBehaviour
{
    public GameObject attackSpell;
    public float telekenisisStrength = 0.05f;

    private GameManager gameManager;
    private GameObject[] oobles;
    private OVRGrabbable grabbable;
    private bool isActive = false;

    [HideInInspector]
    public int inHand = 0;
    private bool wasActive;

    public float attackCooldown;
    private float cooldownTimer;
    private bool cooldown = false;

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        oobles = gameManager.oobles;
        grabbable = GetComponent<OVRGrabbable>();
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

        inHand = 0;
        if (grabbable.isGrabbed)
        {
            if (grabbable.grabbedBy.tag == "RightController")
                inHand = 1;
            else if (grabbable.grabbedBy.tag == "LeftController")
                inHand = 2;
        }

        bool primaryValue = false;
        isActive = false;
        if (inHand == 1)
        {
            if (gameManager.rightController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondaryValue) && secondaryValue)
            {
                isActive = true;
            }

            if (!cooldown && gameManager.rightController.TryGetFeatureValue(CommonUsages.primaryButton, out primaryValue) && primaryValue)
            {
                Rigidbody rb = Instantiate(attackSpell, transform.position + transform.forward * 0.21f, Quaternion.identity).GetComponent<Rigidbody>();
                rb.AddForce(transform.forward * 500);
                cooldown = true;
            }
        }
        else if (inHand == 2)
        {
            if (gameManager.leftController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondaryValue) && secondaryValue)
            {
                isActive = true;
            }

            if (!cooldown && gameManager.leftController.TryGetFeatureValue(CommonUsages.primaryButton, out primaryValue) && primaryValue)
            {
                Rigidbody rb = Instantiate(attackSpell, transform.position + transform.forward * 0.21f, Quaternion.identity).GetComponent<Rigidbody>();
                rb.AddForce(transform.forward * 500);
                cooldown = true;
            }
        }

        if (isActive)
        {
            foreach(GameObject ooble in oobles)
            {
                Rigidbody rb = ooble.GetComponent<Rigidbody>();
                rb.useGravity = true;

                Vector3 displacement = transform.position - ooble.transform.position;
                float magnitude = displacement.magnitude;
                Vector3 direction = displacement / magnitude;
                float force = telekenisisStrength / magnitude;

                if (magnitude < 3f)
                    rb.useGravity = false;

                rb.AddForce(direction * force);
            }

            Debug.Log("Active");
        }

        if (isActive != wasActive && wasActive)
        {
            foreach (GameObject ooble in oobles)
            {
                ooble.GetComponent<Rigidbody>().useGravity = true;
            }
        }

        wasActive = isActive;
    }
}
