using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemMenu : MonoBehaviour
{
    public static bool active = true;
    void Start()
    {    
        ControllerSelection.OVRPointerVisualizer.active = active;
    }

    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.SecondaryThumbstick))
        {
            active = !active;
            ControllerSelection.OVRPointerVisualizer.active = active;
        }
    }
}
