using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// DiegeticSlider is a component which will move between two predefined locations
/// when a draggable is moved.
/// </summary>
public class DiegeticSlider : MonoBehaviour
{
    [Header("Slider Values")]

    [Tooltip("The current/initial value of the slider.")]
    [SerializeField] private float currentValue = 0;
    [Tooltip("The minimum value of the slider.")]
    public float minimumValue = 0;
    [Tooltip("The maximum value of the slider.")]
    public float maximumValue = 1;

    [Header("Slider Components")]

    [Tooltip("The slidable part will slide betweeen the slide start and end points.")]
    public Transform moveablePart;
    [Tooltip("The minimum location of the moveable part.")]
    public Transform slideStartPoint;
    [Tooltip("The maximum location of the moveable part.")]
    public Transform slideEndPoint;
    [Tooltip("The location the grabbable will reset to when it is no longer grabbed.")]
    public Transform grabbableResetPoint;

    public Outline inRangeOutlineHighlighter;

    [Header("Slider Properties")]
    public bool isGrabbed;
    public bool isLocked;
    private bool smoothing = true;
    public bool resetWhenNotGrabbed;
    public float maximumDistanceBeforeForcedDrop = 0.5f;
    public OVRGrabbableExtended grabbable;

    [Space(20)]

    [Tooltip("Called whenever the value of the slider changes.")]
    public UnityEvent<float> onValueChanged;
    [Tooltip("Called whenever the value of the slider changes.")]
    public UnityEvent<bool> setToMinimumValue;
    [Tooltip("Called whenever the value of the slider changes.")]
    public UnityEvent<bool> setToMaximumValue;
    
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
            if (!isLocked)
            {
                // Clamp the value so that it is between the minimum and maximum value.
                value = Mathf.Clamp(value, minimumValue, maximumValue);

                // If the new value is different to the old value, invoke the value changed event.
                if (value != CurrentValue)
                    onValueChanged.Invoke(value);

                // Invoke the change event if the value has changed FROM or TO the minimum value.
                if (value != CurrentValue && (value == minimumValue || CurrentValue == minimumValue))
                    setToMinimumValue.Invoke(value == minimumValue);

                // Invoke the change event if the value has changed FROM or TO the maximum value.
                if (value != CurrentValue && (value == maximumValue || CurrentValue == maximumValue))
                    setToMaximumValue.Invoke(value == maximumValue);

                // Apply the updated value.
                currentValue = value;

                // Update the slider to match the new value.
                UpdateSliderVisual();

            }
        }
    }

    /// <summary>
    /// Update the slider position when the slider is created.
    /// </summary>
    private void Start ()
    {
        CurrentValue = currentValue;
        onValueChanged.Invoke(currentValue);
    }

    /// <summary>
    /// If the slider is being grabbed, calculate the new value of the slider by mapping
    /// the location of the draggable onto the slideable line.
    /// </summary>
    private void Update()
    {
        if (isGrabbed)
        {
            // Find the closest point to the grabbable on the line between the start/end points.
            Vector3 point = ClosestPoint(
                slideStartPoint.transform.position, 
                slideEndPoint.transform.position, 
                grabbable.transform.position);

            // Update the slider value based on the current grabbable position.
            float distToStart = Vector3.Distance(point, slideStartPoint.position);
            float totalDist = Vector3.Distance(slideStartPoint.position, slideEndPoint.position);
            CurrentValue = minimumValue + (distToStart / totalDist) * (maximumValue - minimumValue);

            if (Vector3.Distance(grabbable.transform.position, grabbableResetPoint.transform.position) >
            maximumDistanceBeforeForcedDrop) grabbable.grabbedBy.ForceRelease(grabbable);
        }
        else
        {
            ResetGrabbableTransform();
        }
        UpdateSliderVisual();
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
    /// Update the slider transform to match the stored value.
    /// </summary>
    private void UpdateSliderVisual ()
    {
        // Calculate the new position the slider should be moved to.
        Vector3 newSliderPosition = slideStartPoint.position;
        float sliderRatio = (CurrentValue - minimumValue) / (maximumValue - minimumValue);
        newSliderPosition += sliderRatio * (slideEndPoint.position - slideStartPoint.position);

        // If smoothing is enabled, lerp between the current position and the new position.
        if (smoothing)
        {
            moveablePart.position = Vector3.Lerp(
                moveablePart.position, 
                newSliderPosition, 
                Time.deltaTime * 10.0f);
        }

        // Otherwise, immediately move the new position.
        else
        {
            moveablePart.transform.position = newSliderPosition;
        }
    }

    /// <summary>
    /// Locks the component, preventing the component value from changing.
    /// </summary>
    public void Lock()
    {
        isLocked = true;
    }

    /// <summary>
    /// Unlocks the component, allowing the component value to change.
    /// </summary>
    public void Unlock()
    {
        isLocked = false;
    }

    /// <summary>
    /// Sets the lock state of the component, determining whether the value can change.
    /// </summary>
    public void SetLockState(bool lockState)
    {
        isLocked = lockState;
    }

    /// <summary>
    /// Actions to perform when a child grabbable begins. Override as needed. 
    /// </summary>
    private void OnGrabBegin()
    {
        isGrabbed = true;
    }

    /// <summary>
    /// Actions to perform when a child grabbable ends. Override as needed.
    /// </summary>
    private void OnGrabEnd()
    {
        isGrabbed = false;
        if (resetWhenNotGrabbed)
        {
            CurrentValue = minimumValue;
        }
    }

    /// <summary>
    /// Called when an OVRGrabber enters the grab range of this component.
    /// </summary>
    protected virtual void OnEnterGrabRange ()
    {
        inRangeOutlineHighlighter.enabled = true;
    }

    /// <summary>
    /// Called when an OVRGrabber exits the grab range of this component.
    /// </summary>
    protected virtual void OnExitGrabRange ()
    {
        inRangeOutlineHighlighter.enabled = false;
    }

    protected virtual void ResetGrabbableTransform()
    {
        Rigidbody r = grabbable.GetComponent<Rigidbody>();
        if (r != null)
        {
            r.MovePosition(grabbableResetPoint.position);
            r.MoveRotation(grabbableResetPoint.rotation);
        }
        else
        {
            grabbable.transform.position = grabbableResetPoint.position;
            grabbable.transform.rotation = grabbableResetPoint.rotation;
        }
    }

    /// <summary>
    /// Find the closest point on the line between A and B to the specified point P.
    /// </summary>
    private Vector3 ClosestPoint(Vector3 a, Vector3 b, Vector3 p)
    {
        Vector3 v1 = p - a;
        Vector3 v2 = (b - a).normalized;
        float distance = Vector3.Distance(a, b);
        float t = Vector3.Dot(v2, v1);
        if (t <= 0) return a;
        if (t >= distance) return b;
        return (a + (v2 * t));
    }
}
