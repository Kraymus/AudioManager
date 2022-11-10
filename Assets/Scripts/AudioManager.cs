using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

[ExecuteInEditMode]
public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private Transform playerTransform; // Used as the parent for the player audio pool objects
    [SerializeField, Range(0, 1)]
    private float masterVolume = 1f;
    [SerializeField]
    private List<AudioGroup> audioGroups;

#if UNITY_EDITOR
    private AudioSource editorAudioSource;
#endif
    private Dictionary<string, AudioGroup> audioGroupDict = new Dictionary<string, AudioGroup>();
    private ObjectPool<GameObject> audioPoolPlayer = new ObjectPool<GameObject>(OnCreatePlayerAudio, OnTakeAudio, OnReleaseAudio, OnDestroyAudio);
    private ObjectPool<GameObject> audioPoolPositional = new ObjectPool<GameObject>(OnCreatePositionalAudio, OnTakeAudio, OnReleaseAudio, OnDestroyAudio);

    private Dictionary<GameObject, float> audioTimerPlayer = new Dictionary<GameObject, float>();
    private Dictionary<GameObject, float> audioTimerPositional = new Dictionary<GameObject, float>();

    private static AudioManager instance = null;
    public static AudioManager Instance 
    { 
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<AudioManager>();
            }
            return instance;
        }
    }

    private void Start()
    {
        BuildAudioGroupDict();
    }

    private void Update()
    {
        if (audioTimerPlayer.Count > 0)
        {
            var audioTimerPlayerKeys = audioTimerPlayer.Keys.ToList();
            for (int i = audioTimerPlayerKeys.Count - 1; i >= 0; i--)
            {
                var key = audioTimerPlayerKeys[i];
                audioTimerPlayer[key] -= Time.deltaTime;
                if (audioTimerPlayer[key] <= 0)
                {
                    audioPoolPlayer.Release(key);
                    audioTimerPlayer.Remove(key);
                }
            }
        }

        if (audioTimerPositional.Count > 0)
        {
            var audioTimerPositionalKeys = audioTimerPositional.Keys.ToList();
            for (int i = audioTimerPositionalKeys.Count - 1; i >= 0; i--)
            {
                var key = audioTimerPositionalKeys[i];
                audioTimerPositional[key] -= Time.deltaTime;
                if (audioTimerPositional[key] <= 0)
                {
                    audioPoolPositional.Release(key);
                    audioTimerPositional.Remove(key);
                }
            }
        }
    }

#if UNITY_EDITOR
    private void InitializeEditorAudioSource()
    {
        if (editorAudioSource != null)
            return;

        editorAudioSource = GetComponent<AudioSource>();
        if (editorAudioSource == null)
            editorAudioSource = gameObject.AddComponent<AudioSource>();

        //audioSource.hideFlags = HideFlags.HideInInspector;
        editorAudioSource.playOnAwake = false;
    }

    public void PlayInEditor(string audioGroupName)
    {
        if (!Application.isPlaying)
            BuildAudioGroupDict();

        InitializeEditorAudioSource();
        PlayFromSource(audioGroupName, editorAudioSource);
    }

    public void PlayAudioGroupSegmentInEditor(string audioGroupName, int audioGroupSegmentIndex)
    {
        if (!Application.isPlaying)
            BuildAudioGroupDict();

        AudioGroup audioGroup = audioGroupDict[audioGroupName];
        InitializeEditorAudioSource();
        PlayAudioGroupSegment(audioGroup.GetSegments()[audioGroupSegmentIndex], audioGroup.GetVolume(), editorAudioSource);
    }
#endif

    public void Play(string audioGroupName)
    {
        GameObject audioObject = audioPoolPlayer.Get();
        audioObject.transform.position = playerTransform.position;
        audioObject.transform.parent = playerTransform;
        AudioClip clip = PlayFromSource(audioGroupName, audioObject.GetComponent<AudioSource>());
        if (clip == null)
        {
            audioPoolPlayer.Release(audioObject);
        }
        else
        {
            audioTimerPlayer.Add(audioObject, clip.length);
        }
    }

    public void Play(string audioGroupName, Vector3 position)
    {
        GameObject audioObject = audioPoolPositional.Get();
        audioObject.transform.position = position;
        audioObject.transform.parent = transform;
        AudioClip clip = PlayFromSource(audioGroupName, audioObject.GetComponent<AudioSource>());
        if (clip == null)
        {
            audioPoolPositional.Release(audioObject);
        }
        else
        {
            audioTimerPositional.Add(audioObject, clip.length);
        }
    }

    private AudioClip PlayFromSource(string audioGroupName, AudioSource audioSource)
    {
        AudioGroup audioGroup = audioGroupDict[audioGroupName];
        if (audioGroup == null)
        {
            Debug.LogWarning("No Audio Group with the name: " + audioGroupName);
            return null;
        }

        List<AudioGroupSegment> audioGroupSegments = audioGroup.GetSegments();
        if (audioGroupSegments.Count == 0)
        {
            Debug.LogWarning("Audio Group Has no segments to play");
            return null;
        }

        float audioGroupVolume = audioGroup.GetVolume();
        AudioGroupType audioGroupType = audioGroup.GetAudioGroupType();

        if (audioGroupType == AudioGroupType.Random)
        {
            // Calculate the total weight of all the segments
            int totalWeight = 0;
            for (int i = 0; i < audioGroupSegments.Count; i++)
                totalWeight += audioGroupSegments[i].GetWeight();

            // Get a random number within that weight
            int randomNumber = Random.Range(0, totalWeight);

            // Find the right segment to play corresponding to the random number.
            int weightSoFar = 0;
            int weightedRandomIndex = -1;
            for (int i = 0; i < audioGroupSegments.Count; i++)
            {
                weightSoFar += audioGroupSegments[i].GetWeight();
                if (randomNumber < weightSoFar)
                {
                    weightedRandomIndex = i;
                    break;
                }
            }

            return PlayAudioGroupSegment(audioGroupSegments[weightedRandomIndex], audioGroupVolume, audioSource);
        }
        else // Sequence
        {
            int audioGroupResetTime = audioGroup.GetResetTime();
            return PlayAudioGroupSegment(audioGroup.SequenceStep(), audioGroup.GetVolume(), audioSource);
        }
    }

    private AudioClip PlayAudioGroupSegment(AudioGroupSegment audioGroupSegment, float audioGroupVolume, AudioSource audioSource)
    {
        audioSource.clip = audioGroupSegment.GetAudioClip();
        float volume = audioGroupSegment.GetVolume();
        float pitch = audioGroupSegment.GetPitch();
        float randomVolume = audioGroupSegment.GetRandomVolume();
        float randomPitch = audioGroupSegment.GetRandomPitch();
        float finalVolume = Random.Range(volume - randomVolume, volume + randomVolume) * audioGroupVolume;
        float finalPitch = Random.Range(pitch - randomPitch, pitch + randomPitch);
        audioSource.volume = finalVolume;
        audioSource.pitch = finalPitch;
        audioSource.Play();
        return audioSource.clip;
    }

    private void BuildAudioGroupDict()
    {
        audioGroupDict.Clear();
        foreach (AudioGroup audioGroup in audioGroups)
            audioGroupDict.Add(audioGroup.GetName(), audioGroup);
    }

    #region PoolFunctions
    private static GameObject OnCreatePlayerAudio()
    {
        GameObject soundGameObject = new GameObject("Audio");
        soundGameObject.AddComponent<AudioSource>();

        return soundGameObject;
    }

    private static GameObject OnCreatePositionalAudio()
    {
        GameObject soundGameObject = new GameObject("Audio");
        soundGameObject.AddComponent<AudioSource>();

        return soundGameObject;
    }

    private static void OnTakeAudio(GameObject obj)
    {
        obj.SetActive(true);
    }

    private static void OnReleaseAudio(GameObject obj)
    {
        obj.SetActive(false);
    }

    private static void OnDestroyAudio(GameObject obj)
    {
        Destroy(obj);
    }
    #endregion
}
