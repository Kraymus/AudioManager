using Kraymus.AudioManager;
using UnityEngine;

public class AudioManagerTester : MonoBehaviour
{
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
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            AudioManager.Instance.PlayMusic("Music 1", 5, 5);
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            AudioManager.Instance.FadeOutMusic(5);
        }
    }
}
