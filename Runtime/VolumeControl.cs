using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Kraymus.AudioManager
{
    public class VolumeControl : MonoBehaviour
    {
        [SerializeField] private string volumeParameter = "MasterVolume";
        [SerializeField] private AudioMixer mixer;
        [SerializeField] private Slider slider;
        [SerializeField] private Toggle toggle;
        [SerializeField] private float multiplier = 30f;

        private void Awake()
        {
            slider.onValueChanged.AddListener(OnSliderValueChanged);
            toggle.onValueChanged.AddListener(OnToggleValueChanged);
        }

        private void OnDisable()
        {
            PlayerPrefs.SetFloat(volumeParameter, slider.value);
            PlayerPrefs.SetInt(volumeParameter + "Toggle", toggle.isOn ? 1 : 0);
        }

        private void Start()
        {
            slider.value = PlayerPrefs.GetFloat(volumeParameter, slider.value);
            toggle.isOn = PlayerPrefs.GetInt(volumeParameter + "Toggle", toggle.isOn ? 1 : 0) == 1;
        }

        private void OnSliderValueChanged(float value)
        {
            SetMixerFloat(value);
        }

        private void OnToggleValueChanged(bool enableSound)
        {
            slider.interactable = enableSound;
            if (enableSound)
                SetMixerFloat(slider.value);
            else
                MuteMixer();
        }

        private void SetMixerFloat(float value)
        {
            mixer.SetFloat(volumeParameter, Mathf.Log10(value) * multiplier);
        }

        private void MuteMixer()
        {
            mixer.SetFloat(volumeParameter, -80f);
        }
    }
}
