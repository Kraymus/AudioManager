using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraymus.AudioManager
{
    [System.Serializable]
    public class AudioGroupSegment
    {
        [SerializeField] private AudioSegment audioSegment;
        [Range(1, 100)]
        [SerializeField] private int weight = 1;

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

        public int GetWeight()
        {
            return weight;
        }

        public AudioSegment GetAudioSegment()
        {
            return audioSegment;
        }
    }
}
