using UnityEngine;

namespace Kraymus.AudioManager
{
    [System.Serializable]
    public class NamedAudioSegment
    {
        [SerializeField] private string name;
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
            return name;
        }

        public AudioSegment GetAudioSegment()
        {
            return audioSegment;
        }
    }
}
