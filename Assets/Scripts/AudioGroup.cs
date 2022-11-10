using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AudioGroupType
{
    Random,
    Sequence
}

[System.Serializable]
public class AudioGroup
{
    [SerializeField]
    private string name;
    [SerializeField, Range(0, 1)]
    private float volume;
    [SerializeField]
    private List<AudioGroupSegment> segments = new List<AudioGroupSegment>();
    [SerializeField]
    private AudioGroupType type;
    [SerializeField]
    private int resetTime;
    [SerializeField]
    private int currentSequence = 0;

    public string GetName()
    {
        return name;
    }

    public float GetVolume()
    {
        return volume;
    }

    public AudioGroupType GetAudioGroupType()
    {
        return type;
    }

    public int GetResetTime()
    {
        return resetTime;
    }

    public List<AudioGroupSegment> GetSegments()
    {
        return segments;
    }

    public AudioGroupSegment SequenceStep()
    {
        if (currentSequence >= segments.Count)
            currentSequence = 0;
        AudioGroupSegment segment = segments[currentSequence];
        Debug.Log("Sequence " + currentSequence);
        currentSequence++;
        return segment;
    }

    public void ResetSequence()
    {
        currentSequence = 0;
    }
}
