using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TimerType
{
    FadeOut,
    FadeIn
}

public class MusicTimer
{
    private GameObject audioObject = null;
    private AudioSource audioSource;
    private float timeLeft = 0;
    private float originalVolume;
    private float totalTime;
    private float delay;
    private TimerType timerType;

    public MusicTimer(TimerType timerType, float time, GameObject audioObject, float delay = 0f)
    {
        this.delay = delay;
        this.timerType = timerType;
        totalTime = timeLeft = time;

        this.audioObject = audioObject;
        audioSource = audioObject.GetComponent<AudioSource>();
        originalVolume = audioSource.volume;
        if (timerType == TimerType.FadeIn)
            audioSource.volume = 0;
    }

    // Returns a bool that informs if the timer is done
    public bool Tick(float deltaTime)
    {
        if (delay > 0f)
        {
            delay -= deltaTime;
            return false;
        }
        else if (timeLeft > 0f)
        {
            timeLeft -= deltaTime;
            if (timerType == TimerType.FadeOut)
                audioSource.volume -= (deltaTime / totalTime) * originalVolume;
            else if (timerType == TimerType.FadeIn)
                audioSource.volume += (deltaTime / totalTime) * originalVolume;
            return false;
        }
        return true;
    }

    public TimerType GetTimerType()
    {
        return timerType;
    }

    public GameObject GetAudioObject()
    {
        return audioObject;
    }

    public void Reverse()
    {
        if (timerType == TimerType.FadeOut)
        {
            Debug.LogWarning("Reverse on fading out");
            return;
        }
        originalVolume = audioSource.volume;
        totalTime = timeLeft;
        timerType = TimerType.FadeOut;
    }
}
