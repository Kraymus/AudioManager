using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AudioGroupSegment
{
    [SerializeField] private AudioClip audioClip;
    [Range(0, 1)]
    [SerializeField] private float volume = 1;
    [Range(0, 3)]
    [SerializeField] private float pitch = 1;
    [Range(0, 1)]
    [SerializeField] private float randomVolume = 0;
    [Range(0, 1)]
    [SerializeField] private float randomPitch = 0;
    [Range(1, 100)]
    [SerializeField] private int weight = 1;

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

    public float GetRandomVolume()
    {
        return randomVolume;
    }

    public float GetRandomPitch()
    {
        return randomPitch;
    }

    public int GetWeight()
    {
        return weight;
    }
}
