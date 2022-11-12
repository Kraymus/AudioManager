using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraymus.AudioManager
{
    [System.Serializable]
    public class Music
    {
        [SerializeField] private string name;
        [SerializeField] private AudioClip audioClip;
        [SerializeField, Range(0, 1)] private float volume;

        public string GetName()
        {
            return name;
        }

        public AudioClip GetAudioClip()
        {
            return audioClip;
        }

        public float GetVolume()
        {
            return volume;
        }
    }
}
