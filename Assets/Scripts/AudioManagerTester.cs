using Kraymus.AudioManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerTester : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            AudioManager.Instance.Play(AudioCategory.Group, "Audio Group 1");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            AudioManager.Instance.Play(AudioCategory.Group, "Audio Group 2", Vector3.zero);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            AudioManager.Instance.Play(AudioCategory.Segment, "Segment 1", Vector3.zero);
        }
    }
}
