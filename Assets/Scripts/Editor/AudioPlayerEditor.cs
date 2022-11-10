using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(AudioPlayer))]
public class AudioPlayerEditor : Editor
{
    private PropertyField audioClipPropertyField { get; set; }
    private SerializedProperty audioClipProperty { get; set; }
    private PropertyField volumePropertyField { get; set; }
    private SerializedProperty volumeProperty { get; set; }
    private PropertyField pitchPropertyField { get; set; }
    private SerializedProperty pitchProperty { get; set; }


    private Button playAudioButton { get; set; }
    private Button stopAudioButton { get; set; }

    private AudioSource audioSource;
    private AudioPlayer audioPlayerScript;

    public override VisualElement CreateInspectorGUI()
    {
        SetupAudioSource();
        VisualElement myInspector = new VisualElement();

        audioClipProperty = serializedObject.FindProperty("audioClip");
        audioClipPropertyField = new PropertyField(audioClipProperty);
        myInspector.Add(audioClipPropertyField);

        volumeProperty = serializedObject.FindProperty("volume");
        volumePropertyField = new PropertyField(volumeProperty);
        myInspector.Add(volumePropertyField);

        pitchProperty = serializedObject.FindProperty("pitch");
        pitchPropertyField = new PropertyField(pitchProperty);
        myInspector.Add(pitchPropertyField);

        playAudioButton = new Button(PlayAudio);
        playAudioButton.text = "Play";
        myInspector.Add(playAudioButton);

        stopAudioButton = new Button(StopAudio);
        stopAudioButton.text = "Stop";
        myInspector.Add(stopAudioButton);

        return myInspector;
    }

    private void PlayAudio()
    {
        audioSource.clip = audioPlayerScript.GetAudioClip();
        audioSource.volume = audioPlayerScript.GetVolume();
        audioSource.pitch = audioPlayerScript.GetPitch();
        audioSource.Play();
    }

    private void StopAudio()
    {
        audioSource.Stop();
    }

    private void SetupAudioSource()
    {
        audioPlayerScript = (AudioPlayer)target;
        audioSource = audioPlayerScript.gameObject.GetComponent<AudioSource>();

        // Adds an Audio Source to the gameObject this script is on if its not already there (used for previewing audio only) 
        // * Hide flags hides it from the inspector so you don't notice it there *
        if (audioSource)
        {
            //audioSource.hideFlags = HideFlags.HideInInspector;
            audioSource.playOnAwake = false;
        }
        else
        {
            audioPlayerScript.gameObject.AddComponent<AudioSource>();
            audioSource = audioPlayerScript.gameObject.GetComponent<AudioSource>();
            //audioSource.hideFlags = HideFlags.HideInInspector;
            audioSource.playOnAwake = false;
        }
    }
}
