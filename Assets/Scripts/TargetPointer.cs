using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Makes the pointer object point towards a target and follow another targets position
public class TargetPointer : MonoBehaviour
{
    public Transform pivot;
    public Transform lookTarget;
    public Transform positionTarget;

    private void Awake()
    {
        lookTarget = GameObject.FindGameObjectWithTag("CollectionUnit").transform;
    }

    private void Update()
    {
        pivot.LookAt(lookTarget);
        pivot.rotation = Quaternion.Euler(0, pivot.rotation.eulerAngles.y, 0);

        transform.position = positionTarget.position + new Vector3(0, 0.25f, 0);
    }
}
