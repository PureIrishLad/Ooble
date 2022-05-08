using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// DiegeticRotator is a component which will rotate between the given constraints
/// on the specified axis when a draggable is moved.
/// </summary>
public class DiegeticRotator : MonoBehaviour
{
    [Header("Current slider value. Don't edit here.")]
    public float currentValue = 0;

    [Header("Slider Values")]

    [Tooltip("The initial value of the component.")]
    public float initialValue = 0;
    [Tooltip("The minimum value of the slider.")]
    public float minimumValue = 0;
    [Tooltip("The maximum value of the slider.")]
    public float maximumValue = 1;

    [Header("Rotator Components")]
    [Tooltip("The component which will be rotated.")]
    public Transform rotatablePart;
    [Tooltip("The location the grabbable will reset to when it is no longer grabbed.")]
    public Transform grabbableResetPoint;
    [Tooltip("The axis on which the object will rotate.")]
    public RotationAxis rotationAxis;
    [Tooltip("The minimum angle of rotation for the object.")]
    public float minimumAngle = 0;
    [Tooltip("The maximum angle of rotation for the object.")]
    public float maximumAngle = 360;
    [Tooltip("The specified object will be outlined when the component can be grabbed. Can be null.")]
    public Outline inRangeOutlineHighlighter;

    [Header("Rotator Properties")]
    [Tooltip("Is the object currently being grabbed?")]
    public bool isGrabbed;
    [Tooltip("Is the rotation of the object currently locked?")]
    public bool isLocked;
    [Tooltip("Should this object reset to its initial value when it is no longer grabbed?")]
    public bool resetWhenNotGrabbed;
    [Tooltip("How smoothly should the object rotate to its new location? 0 = no delay.")]
    [Range(0.0f, 1.0f)]
    public float smoothing = 0;
    public float maximumDistanceBeforeForcedDrop = 0.5f;

    [Space(20)]

    [Tooltip("Called whenever the value of the slider changes.")]
    public UnityEvent<float> onValueChanged;
    [Tooltip("Called when the slider changes into or out of the minimum value.")]
    public UnityEvent<bool> setToMinimumValue;
    [Tooltip("Called when the slider changes into or out of the maximum value.")]
    public UnityEvent<bool> setToMaximumValue;
    [Tooltip("Called when the user starts grabbing the object to rotate it.")]
    public UnityEvent onGrabStarted;
    [Tooltip("Called when the user sops grabbing the object to rotate it.")]
    public UnityEvent onGrabFinished;


    // Cache a reference to the grabbable.
    public OVRGrabbableExtended grabbable;

    // Where is the object currently trying to rotate towards?
    protected float desiredDegrees = 0;


    /// <summary>
    /// Property to correctly handle external updates to the current value.
    /// </summary>
    public float CurrentValue
    {
        get
        {
            return currentValue;
        }
        set
        {
            // Clamp the value so that it is between the minimum and maximum value.
            value = Mathf.Clamp(value, minimumValue, maximumValue);

            // If the new value is different to the old value, invoke the value changed event.
            if (value != CurrentValue)
                onValueChanged.Invoke(value);

            // Invoke the change event if the value has changed FROM or TO the minimum value.
            if (value != CurrentValue && (value == minimumValue || CurrentValue == minimumValue)) {
                setToMinimumValue.Invoke(value == minimumValue);
            }

            // Invoke the change event if the value has changed FROM or TO the maximum value.
            //if (value != CurrentValue && (value == maximumValue || CurrentValue == maximumValue)) {
            if (value != CurrentValue && (value == maximumValue)) {
                setToMaximumValue.Invoke(value == maximumValue);
            }

            // Apply the updated value.
            currentValue = value;

            // Update the rotator visual to match the new value.
            float ratio = ((CurrentValue - minimumValue) / (maximumValue - minimumValue));
            float newDesiredDegrees = minimumAngle + ratio * (maximumAngle - minimumAngle);

            //if (Mathf.Abs(newDesiredDegrees - desiredDegrees) < 20.0f)
            //{
                desiredDegrees = newDesiredDegrees;
            //}
        }
    }

    public enum RotationAxis
    {
      XAxis,
      YAxis,
      ZAxis
    };

    /// <summary>
    /// Update the rotator value when it is first created.
    /// </summary>
    protected virtual void Start()
    {
        CurrentValue = initialValue;
        //onValueChanged.Invoke(CurrentValue);
        ResetGrabbableTransform();
    }

    /// <summary>
    /// If the grabbable is being grabbed, calculate the new rotation of the component
    /// based on the position of the grabbable relative to the component.
    /// </summary>
    protected virtual void Update()
    {
        if (isGrabbed && !isLocked)
        {
            // Calculate the closest point on the aligned axis.
            Vector3 normal = Vector3.zero;
            if (rotationAxis == RotationAxis.XAxis) normal = transform.right;
            if (rotationAxis == RotationAxis.YAxis) normal = transform.up;
            if (rotationAxis == RotationAxis.ZAxis) normal = transform.forward;
            Plane plane = new Plane(normal, rotatablePart.position);
            Vector3 point = plane.ClosestPointOnPlane(grabbable.transform.position);

            // Find the aligned vector used to calculate angles.
            Vector3 diff = transform.InverseTransformPoint(point) - 
                transform.InverseTransformPoint(rotatablePart.position);

            // Calculate the angle.
            float degs = 0;
            if (rotationAxis == RotationAxis.XAxis)
                degs = -Mathf.Atan2(diff.y, diff.z) * Mathf.Rad2Deg;
            else if (rotationAxis == RotationAxis.YAxis)
                degs = Mathf.Atan2(diff.x, diff.z) * Mathf.Rad2Deg;
            else if (rotationAxis == RotationAxis.ZAxis)
                degs = -Mathf.Atan2(diff.x, diff.y) * Mathf.Rad2Deg;

            // Clamp the final angle to the needed bounds.
            while (degs < 0) degs += 360;
            while (degs > 360) degs -= 360;
            float newDesiredDegrees = Mathf.Clamp(degs, minimumAngle, maximumAngle);
            if (Mathf.Abs(newDesiredDegrees - desiredDegrees) < 20.0f)
            {
                desiredDegrees = newDesiredDegrees;
            }

            // Update the value on the rotator.
            float ratio = (desiredDegrees - minimumAngle) / (maximumAngle - minimumAngle);
            CurrentValue = minimumValue + ratio * (maximumValue - minimumValue);

            if (Vector3.Distance(grabbable.transform.position, grabbableResetPoint.transform.position) > 
                maximumDistanceBeforeForcedDrop) grabbable.grabbedBy.ForceRelease(grabbable);
        }

        Quaternion targetRotation = Quaternion.Euler(
                rotationAxis == RotationAxis.XAxis ? desiredDegrees : 0,
                rotationAxis == RotationAxis.YAxis ? desiredDegrees : 0,
                rotationAxis == RotationAxis.ZAxis ? desiredDegrees : 0
            );

        // Handle final smoothing.
        if (smoothing > 0)
        {
            rotatablePart.localRotation = Quaternion.Lerp(rotatablePart.localRotation, targetRotation, Time.deltaTime * (1.1f - smoothing) * 10);
        }
        else
        {
            rotatablePart.localRotation = targetRotation;
        }

        if (!isGrabbed) ResetGrabbableTransform();
        
    }

    /// <summary>
    /// When the component is enabled, assign the grab listeners.
    /// </summary>
    private void OnEnable()
    {
        grabbable = GetComponentInChildren<OVRGrabbableExtended>();
        grabbable.OnGrabBegin.AddListener(OnGrabBegin);
        grabbable.OnGrabEnd.AddListener(OnGrabEnd);
        grabbable.OnEnterGrabRange.AddListener(OnEnterGrabRange);
        grabbable.OnExitGrabRange.AddListener(OnExitGrabRange);
    }

    /// <summary>
    /// When the component is disabled, remove the grab listeners.
    /// </summary>
    private void OnDisable()
    {
        grabbable.OnGrabBegin.RemoveListener(OnGrabBegin);
        grabbable.OnGrabEnd.RemoveListener(OnGrabEnd);
        grabbable.OnEnterGrabRange.RemoveListener(OnEnterGrabRange);
        grabbable.OnExitGrabRange.RemoveListener(OnExitGrabRange);
    }

    /// <summary>
    /// Locks the component, preventing the component value from changing.
    /// </summary>
    public virtual void Lock()
    {
        isLocked = true;
    }

    /// <summary>
    /// Unlocks the component, allowing the component value to change.
    /// </summary>
    public virtual void Unlock()
    {
        isLocked = false;
    }

    /// <summary>
    /// Sets the lock state of the component, determining whether the value can change.
    /// </summary>
    public virtual void SetLockState(bool lockState)
    {
        isLocked = lockState;
    }

    /// <summary>
    /// Actions to perform when a child grabbable begins. Override as needed. 
    /// </summary>
    protected virtual void OnGrabBegin()
    {
        isGrabbed = true;
    }

    /// <summary>
    /// Actions to perform when a child grabbable ends. Override as needed.
    /// </summary>
    protected virtual void OnGrabEnd()
    {
        isGrabbed = false;
        ResetGrabbableTransform();
        if (resetWhenNotGrabbed)
        {
            CurrentValue = minimumValue;
        }
    }

    /// <summary>
    /// Called when an OVRGrabber enters the grab range of this component.
    /// </summary>
    protected virtual void OnEnterGrabRange()
    {
        inRangeOutlineHighlighter.enabled = true;
    }

    /// <summary>
    /// Called when an OVRGrabber exits the grab range of this component.
    /// </summary>
    protected virtual void OnExitGrabRange()
    {
        inRangeOutlineHighlighter.enabled = false;
    }

    /// <summary>
    /// Reset the grabbable transform back to its original location.
    /// </summary>
    protected virtual void ResetGrabbableTransform ()
    {
        grabbable.transform.position = grabbableResetPoint.position;
        grabbable.transform.rotation = grabbableResetPoint.rotation;
    }
}
