using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

public class OVRGrabbableExtended : OVRGrabbable
{
    [HideInInspector] public UnityEvent OnGrabBegin;
    [HideInInspector] public UnityEvent OnGrabEnd;
    [HideInInspector] public UnityEvent OnEnterGrabRange;
    [HideInInspector] public UnityEvent OnExitGrabRange;

    public override void GrabBegin(OVRGrabber hand, Collider grabPoint)
    {
        OnGrabBegin.Invoke();
        base.GrabBegin(hand, grabPoint);
        
    }

    public override void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
    {
        base.GrabEnd(linearVelocity, angularVelocity);
        OnGrabEnd.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<OVRGrabber>())
        {
            OnEnterGrabRange.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<OVRGrabber>())
        {
            OnExitGrabRange.Invoke();
        }
    }
}

public class VRUtilities
{
    public static Vector3 ClosestPoint(Vector3 a, Vector3 b, Vector3 p)
    {
        Vector3 v1 = p - a;
        Vector3 v2 = (b - a).normalized;
        float distance = Vector3.Distance(a, b);
        float t = Vector3.Dot(v2, v1);
        if (t <= 0) return a;
        if (t >= distance) return b;
        return (a + (v2 * t));
    }

    public static Vector3 ClosestPoint(Plane plane, Vector3 point)
    {
        Vector3 vectorFind = new Vector3();

        Ray ray = new Ray(point, Vector3.forward);
        float enter = 0.0f;
        if (plane.Raycast(ray, out enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            vectorFind = hitPoint;
        }
        else
        {
            ray = new Ray(point, -Vector3.forward);
            if (plane.Raycast(ray, out enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                vectorFind = hitPoint;
            }
        }
        return vectorFind;
    }
}
