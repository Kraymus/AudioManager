using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraymus.AudioManager
{
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
        private float resetTime;
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

        public float GetResetTime()
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
            currentSequence++;
            return segment;
        }

        public void ResetSequence()
        {
            currentSequence = 0;
        }
    }
}
