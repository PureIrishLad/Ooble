using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// The DiegeticJoystick class allows the user to drag a joystick in two directions within specified boundaries.
/// 
/// The value of the joystick can then be read to control other parts of the experience.
/// </summary>
public class DiegeticJoystick : MonoBehaviour
{
    [Header("Slider Values")]

    [Tooltip("The current/initial value of the slider.")]
    public Vector2 currentValue = Vector2.zero;

    [Tooltip("The part of the joystick which will be rotated.")]
    public Transform rotatablePart;
    [Tooltip("The location the grabbable will reset to when it is no longer grabbed.")]
    public Transform grabbableResetPoint;
    [Tooltip("The joystick will always look towards the guide.")]
    public Transform lockedGuide;
    public Outline inRangeOutlineHighlighter;

    [Header("Rescale the projection plane to make the joystick rotate further.")]
    [Tooltip("Rescale the projection plane to make the joystick rotate further. Move it up and down to control how fast it will reach its maximum bounds.")]
    public Transform projectionPlane;

    [Header("Rotator Properties")]
    public bool isGrabbed;
    public bool isLocked;
    public bool resetWhenNotGrabbed;
    public float maximumDistanceBeforeForcedDrop = 0.5f;
    private OVRGrabbableExtended grabbable;

    [Space(20)]

    // Unity Callback Events
    public UnityEvent<Vector2> onValueChanged;
    public UnityEvent onGrabStarted;
    public UnityEvent onGrabFinished;

    private void Start()
    {
        onValueChanged.Invoke(currentValue);
    }

    /// <summary>
    /// Continually rotate the joystick towards the guide which is locked onto the projection plane.
    /// </summary>
    void Update()
    {
        if (isGrabbed && !isLocked)
        {
            Plane plane = new Plane(projectionPlane.up, projectionPlane.position);
            Vector3 point = plane.ClosestPointOnPlane(grabbable.transform.position);
            lockedGuide.position = point;

            // Find the aligned vector used to calculate angles.

            Vector3 clampedPos = lockedGuide.position;
            clampedPos.x = Mathf.Clamp(lockedGuide.localPosition.x, -0.5f, 0.5f);
            clampedPos.z = Mathf.Clamp(lockedGuide.localPosition.z, -0.5f, 0.5f);
            lockedGuide.localPosition = clampedPos;
            rotatablePart.LookAt(lockedGuide.position);

            // Assign the new value to the slider so it can be read via other scripts.
            Vector2 newValue = new Vector2(clampedPos.x * 2.0f, clampedPos.z * 2.0f);
            if (newValue != currentValue)
            {
                currentValue = newValue;
                onValueChanged.Invoke(currentValue);
            }

            //if (Vector3.Distance(grabbable.transform.position, grabbableResetPoint.transform.position) >
            //maximumDistanceBeforeForcedDrop) grabbable.grabbedBy.ForceRelease(grabbable);
        }
        else
        {
            
        }
        
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
    /// Actions to perform when a child grabbable begins. Override as needed. 
    /// </summary>
    private void OnGrabBegin()
    {
        isGrabbed = true;
        onGrabStarted.Invoke();
    }

    /// <summary>
    /// Actions to perform when a child grabbable ends. Override as needed.
    /// </summary>
    private void OnGrabEnd()
    {
        isGrabbed = false;
        grabbable.transform.position = grabbableResetPoint.position;
        grabbable.transform.rotation = grabbableResetPoint.rotation;
        if (resetWhenNotGrabbed)
        {
            //CurrentValue = minimumValue;
        }
        onGrabFinished.Invoke();
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
}
