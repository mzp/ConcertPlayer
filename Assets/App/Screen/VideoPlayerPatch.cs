using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerPatch : MonoBehaviour
{
    public int SYNC_DURATION = 60 * 60;
    private int frame = 0;
    private VideoPlayer player;

    void Start()
    {
        player = GetComponent<VideoPlayer>();
        // Increate buffer size to prevent audio lag. (https://gametorrahod.com/unity-audio-problems/)
        AudioConfiguration config = AudioSettings.GetConfiguration();
        config.dspBufferSize = 256;
        AudioSettings.Reset(config);
    }

    void Update() {
        if (frame == 0) {
            player.frame = player.frame;
        }
        frame = (frame + 1)  % SYNC_DURATION;
    }
}
