using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemMenu : MonoBehaviour
{
    public static bool active = false;
    void Start()
    {
        
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
