using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayGof : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioSource ambient;

    public void PlaySound()
    {
        ambient.Stop();
        audioSource.Play();
        Invoke("StartAmbientSound", audioSource.clip.length);
    }

    private void StartAmbientSound()
    {
        ambient.Play();
    }
}