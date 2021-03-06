using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Allows the ooble to open the cupboard door when discovered
public class CupboardTrigger : MonoBehaviour
{
    public OobleAI ooble;
    private DiegeticRotator rotator;

    private void Start()
    {
        rotator = transform.parent.GetComponent<DiegeticRotator>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (ooble && collision.gameObject == ooble.gameObject)
        {
            rotator.CurrentValue = rotator.CurrentValue + 1f * Time.deltaTime;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (ooble && collision.gameObject == ooble.gameObject)
        {
            rotator.CurrentValue = rotator.CurrentValue + 1f * Time.deltaTime;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (ooble && collision.gameObject == ooble.gameObject)
        {
            rotator.CurrentValue = rotator.CurrentValue + 1f * Time.deltaTime;
        }
    }
}
