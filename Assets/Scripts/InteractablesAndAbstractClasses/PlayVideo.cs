using Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(AudioSource))]
public class PlayVideo : Interactable
{
    private VideoPlayer videoPlayer;
    public VideoClip videoClip;
    public float videoStartTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Renderer>().enabled = false;
        videoPlayer = gameObject.AddComponent<VideoPlayer>();

        videoPlayer.playOnAwake = false;
        videoPlayer.clip = videoClip;
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
    }

    // Update is called once per frame
    void Update()
    {
    }

    protected override void Interact()
    {
        if (videoPlayer != null)
        {
            if (videoPlayer.isPlaying)
            {
                videoPlayer.Stop();
                GetComponent<Renderer>().enabled = false;
            }
            else
            {
                videoPlayer.time = videoStartTime;
                videoPlayer.Play();
                GetComponent<Renderer>().enabled = true;
            }
        }
    }
}