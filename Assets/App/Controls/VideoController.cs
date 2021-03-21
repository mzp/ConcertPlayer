using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoController : MonoBehaviour
{
    public GameObject ui;
    public VideoPlayer player;
    public Slider slider;

    public void Toggle()
    {
        if (player.isPlaying)
        {
            player.Pause();
        } else
        {
            player.Play();
        }
    }

    public void OnSeekChanged()
    {
        if (!player.isPlaying)
        {
            player.frame = (long)(player.frameCount * slider.normalizedValue);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player.isPlaying)
        {
            slider.value = (float)(player.frame / (player.frameCount * 1.0));
        }


        if (SystemMenu.active)
        {
            if (OVRInput.GetDown(OVRInput.Button.One))
            {
                ui.SetActive(!ui.activeSelf);
            }
        }
        else
        {
            ui.SetActive(false);
        }
    }
 }
