using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NamedAudioSegment
{
    [SerializeField] private string audioName;
    [SerializeField] private AudioSegment audioSegment;

    public AudioClip GetAudioClip()
    {
        return audioSegment.GetAudioClip();
    }

    public float GetVolume()
    {
        return audioSegment.GetVolume();
    }

    public float GetPitch()
    {
        return audioSegment.GetPitch();
    }

    public float GetRandomVolume()
    {
        return audioSegment.GetRandomVolume();
    }

    public float GetRandomPitch()
    {
        return audioSegment.GetRandomPitch();
    }

    public string GetName()
    {
        return audioName;
    }

    public AudioSegment GetAudioSegment()
    {
        return audioSegment;
    }
}
