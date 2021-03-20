using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioBuffer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AudioConfiguration config = AudioSettings.GetConfiguration();
        config.dspBufferSize = 256;
        AudioSettings.Reset(config);
    }
}
