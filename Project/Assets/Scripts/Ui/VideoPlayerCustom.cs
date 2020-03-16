using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoPlayerCustom : MonoBehaviour
{

    [SerializeField] RawImage rawImg = null;
    [SerializeField] VideoPlayer vplayer = null;
    [SerializeField] AudioSource audioSource = null;

    bool hasPlayed = false;

    // Start is called before the first frame update
    void Start()
    {
        //vplayer.Prepare();
        vplayer.SetTargetAudioSource(0, audioSource);
    }

    // Update is called once per frame
    void Update()
    {
        if (vplayer.isPlaying && !hasPlayed)
        {
            rawImg.texture = vplayer.texture;
            hasPlayed = true;
            //audioSource.Play();
        }
    }
}
