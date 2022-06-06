using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Makes the pointer object point towards a target and follow another targets position
public class TargetPointer : MonoBehaviour
{
    public Transform pivot;
    public Transform lookTarget;
    public Transform positionTarget;
    public Vector3 posOffset = new Vector3 (0, 0.25f, 0);
    public Vector3 rotOffset = new Vector3(0, 0, 0);

    private void Awake()
    {
        if (!lookTarget)
            lookTarget = GameObject.FindGameObjectWithTag("CollectionUnit").transform;
    }

    private void Update()
    {
        pivot.LookAt(lookTarget);
        pivot.rotation = Quaternion.Euler(rotOffset.x, pivot.rotation.eulerAngles.y + rotOffset.y, rotOffset.z);

        transform.position = positionTarget.position + posOffset;
    }
}
