using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Stage : MonoBehaviour
{
    public Transform target;
    public Transform origin;
    private Vector3 lastPosition;
    public float distance = 10;
    private float height = 0;
    private bool isDragging = false;

    public void OnDragBegin(BaseEventData data)
    {
        if (SystemMenu.active) { return; }
        ControllerSelection.OVRPointerVisualizer.active = true;
        isDragging = true;
        height = 0;
    }

    public void OnDragEnd(BaseEventData data)
    {
        if (SystemMenu.active) { return; }
        isDragging = false; 
        ControllerSelection.OVRPointerVisualizer.active = false;
    }

    public void OnDrag(BaseEventData data)
    {
        if (SystemMenu.active) { return; }
        this.UpdatePosition(((ControllerSelection.OVRRayPointerEventData)data).pointerCurrentRaycast.worldPosition);
    }
    
    void Update()
    {
        if (isDragging)
        {
            Vector2 primaryAxis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
            distance = Mathf.Clamp(distance + primaryAxis.y / 2, 1, 20);

            Vector2 secondaryAxis = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
            height = Mathf.Clamp((float)height + secondaryAxis.y, -5, 5);

            UpdatePosition(lastPosition);
        }
    }

    void UpdatePosition(Vector3 position)
    {
        lastPosition = position;
        target.position = Vector3.MoveTowards(origin.position, position, distance);
        target.LookAt(new Vector3(0, (float)1.5 + (float)height, 0));
    }
}