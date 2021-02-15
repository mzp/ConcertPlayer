using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenLighController : MonoBehaviour
{
    public GameObject target;
    public MeshRenderer light;
    public Material[] materials;
    private int index = 0;

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
        if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            ChangeToNextColor();
        }
    }

    private void ChangeToNextColor() {
        Debug.Log("Change to next color : " + this.index);
        this.light.material = this.materials[this.index];
        this.index = (this.index + 1) % this.materials.Length;
    }
}