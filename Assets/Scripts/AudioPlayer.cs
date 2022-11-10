using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip audioClip;
    [SerializeField, Range(0, 1)] private float volume = 1;
    [SerializeField, Range(0, 3)] private float pitch = 1;

    public AudioClip GetAudioClip()
    {
        return audioClip;
    }

    public float GetVolume()
    {
        return volume;
    }

    public float GetPitch()
    {
        return pitch;
    }
}
