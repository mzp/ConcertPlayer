using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenLighController : MonoBehaviour
{
    public GameObject target;
    public MeshRenderer penLight;
    public Material[] materials;
    private int index = 1;

    void Start()
    {
        ChangeToNextColor();
    }

    void Update()
    {
        // Track a hand position
        this.transform.position = target.transform.position;
        this.transform.rotation = target.transform.rotation;

        // Color change
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            ChangeToNextColor();
        }
        else if(OVRInput.GetDown(OVRInput.Button.Two))
        {
            ChangeToPrevColor();
        }
    }

    private void ChangeToNextColor() {
        Debug.Log("Change to next color : " + this.index);
        this.penLight.material = this.materials[this.index];
        this.index = (this.index + 1) % this.materials.Length;
    }

    private void ChangeToPrevColor()
    {
        Debug.Log("Change to next color : " + this.index);
        this.penLight.material = this.materials[this.index];
        this.index = (this.index + this.materials.Length - 1) % this.materials.Length;
    }
}