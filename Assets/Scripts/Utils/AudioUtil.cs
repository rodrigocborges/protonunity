using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AudioUtil
{
    
    public static void PlayOneShot(AudioSource source, AudioClip clip){
        source.pitch = Random.Range(0.8f, 1.2f);
        source.PlayOneShot(clip);
    }

}
