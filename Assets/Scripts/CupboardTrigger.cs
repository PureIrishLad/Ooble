using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        if (ooble != null && collision.gameObject == ooble.gameObject)
        {
            rotator.CurrentValue = rotator.CurrentValue + 1f * Time.deltaTime;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (ooble != null && collision.gameObject == ooble.gameObject)
        {
            rotator.CurrentValue = rotator.CurrentValue + 1f * Time.deltaTime;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (ooble != null && collision.gameObject == ooble.gameObject)
        {
            rotator.CurrentValue = rotator.CurrentValue + 1f * Time.deltaTime;
        }
    }
}
