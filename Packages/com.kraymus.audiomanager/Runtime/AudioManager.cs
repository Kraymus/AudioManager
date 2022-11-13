using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

// Tell if a music is fading in when it's asked to fade out

namespace Kraymus.AudioManager
{
    public enum AudioCategory
    {
        Segment,
        Group,
        Music
    }

    [ExecuteInEditMode]
    public class AudioManager : MonoBehaviour
    {
        [SerializeField]
        private Transform playerTransform; // Used as the parent for the player audio pool objects
        [SerializeField, Range(0, 1)]
        private float masterVolume = 1f;
        [SerializeField]
        private List<AudioGroup> audioGroups;
        [SerializeField]
        private List<NamedAudioSegment> audioSegments;
        [SerializeField]
        private List<Music> music;

#if UNITY_EDITOR
        private AudioSource editorAudioSource;
#endif
        private Dictionary<string, AudioGroup> audioGroupDict = new Dictionary<string, AudioGroup>();
        private Dictionary<string, NamedAudioSegment> audioSegmentDict = new Dictionary<string, NamedAudioSegment>();
        private Dictionary<string, Music> musicDict = new Dictionary<string, Music>();
        private ObjectPool<GameObject> audioPool = new ObjectPool<GameObject>(OnCreateAudio, OnTakeAudio, OnReleaseAudio, OnDestroyAudio);
        
        private Dictionary<GameObject, float> audioPoolTimer = new Dictionary<GameObject, float>();
        private Dictionary<AudioGroup, float> sequenceResetTimer = new Dictionary<AudioGroup, float>();

        private GameObject activeMusicObject = null;
        private List<MusicTimer> musicTimers = new List<MusicTimer>();

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

        private void Awake()
        {
            BuildAudioDicts();
        }

        private void Update()
        {
            for (int i = musicTimers.Count - 1; i >= 0; i--)
            {
                MusicTimer musicTimer = musicTimers[i];
                if (musicTimer.Tick(Time.deltaTime))
                {
                    musicTimers.RemoveAt(i);
                    TimerType timerType = musicTimer.GetTimerType();
                    if (timerType == TimerType.FadeOut)
                        audioPool.Release(musicTimer.GetAudioObject());
                }
            }

            if (audioPoolTimer.Count > 0)
            {
                var audioPoolTimerKeys = audioPoolTimer.Keys.ToList();
                for (int i = audioPoolTimerKeys.Count - 1; i >= 0; i--)
                {
                    var key = audioPoolTimerKeys[i];
                    audioPoolTimer[key] -= Time.deltaTime;
                    if (audioPoolTimer[key] <= 0)
                    {
                        audioPool.Release(key);
                        audioPoolTimer.Remove(key);
                    }
                }
            }

            if (sequenceResetTimer.Count > 0)
            {
                var sequenceResetTimerKeys = sequenceResetTimer.Keys.ToList();
                for (int i = sequenceResetTimerKeys.Count - 1; i >= 0; i--)
                {
                    var key = sequenceResetTimerKeys[i];
                    sequenceResetTimer[key] -= Time.deltaTime;
                    if (sequenceResetTimer[key] <= 0)
                    {
                        key.ResetSequence();
                        sequenceResetTimer.Remove(key);
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

        public void PlayInEditor(AudioCategory audioCategory, string audioName, int audioGroupSegmentIndex = -1)
        {
            if (!Application.isPlaying)
                BuildAudioDicts();

            float volumeModifier = 1f;
            InitializeEditorAudioSource();
            if (audioCategory == AudioCategory.Music)
            {
                Music m = GetMusic(audioName);
                if (m != null)
                    PlayMusic(m, editorAudioSource);
            }
            else
            {
                AudioSegment audioSegment = null;
                if (audioCategory == AudioCategory.Group)
                    (audioSegment, volumeModifier) = GetAudioGroupSegment(audioName, audioGroupSegmentIndex);
                else if (audioCategory == AudioCategory.Segment)
                    audioSegment = GetAudioSegment(audioName);

                if (audioSegment != null)
                    PlayAudioSegment(audioSegment, volumeModifier, editorAudioSource);
            }
        }

        public void StopMusicInEditor()
        {
            editorAudioSource.Stop();
        }
#endif

        public void FadeOutMusic(float time)
        {
            if (activeMusicObject != null)
            {
                if (!ReverseExistingMusicTimer(activeMusicObject))
                {
                    MusicTimer musicTimer = new MusicTimer(TimerType.FadeOut, time, activeMusicObject);
                    musicTimers.Add(musicTimer);
                }
                activeMusicObject = null;
            }
        }

        // returns if it exists or not
        private bool ReverseExistingMusicTimer(GameObject musicObject)
        {
            foreach (MusicTimer musicTimer in musicTimers)
            {
                if (musicTimer.GetAudioObject() == musicObject)
                {
                    musicTimer.Reverse();
                    return true;
                }
            }
            return false;
        }

        public void Play(AudioCategory audioCatefory, string audioName)
        {
            Play(audioCatefory, audioName, playerTransform.position, playerTransform);
        }

        public void Play(AudioCategory audioCategory, string audioName, Vector3 position)
        {
            Play(audioCategory, audioName, position, transform);
        }

        private void Play(AudioCategory audioCategory, string audioName, Vector3 position, Transform parent)
        {
            if (audioCategory == AudioCategory.Music)
            {
                PlayMusic(audioName, position, parent);
            }
            else
            {
                AudioSegment audioSegment = null;
                float volumeModifier = 1f;
                if (audioCategory == AudioCategory.Group)
                {
                    (audioSegment, volumeModifier) = GetAudioGroupSegment(audioName);
                }
                else if (audioCategory == AudioCategory.Segment)
                {
                    audioSegment = GetAudioSegment(audioName);
                }

                if (audioSegment != null)
                {
                    GameObject audioObject = audioPool.Get();
                    audioObject.transform.position = position;
                    audioObject.transform.parent = parent;
                    PlayAudioSegment(audioSegment, volumeModifier, audioObject.GetComponent<AudioSource>());
                    audioPoolTimer.Add(audioObject, audioSegment.GetAudioClip().length);
                }
            }
        }

        public void PlayMusic(string audioName, float fadeOutTime = 0f, float fadeInTime = 0f, float fadeInDelay = 0f)
        {
            PlayMusic(audioName, playerTransform.position, playerTransform, fadeOutTime, fadeInTime, fadeInDelay);
        }

        public void PlayMusic(string audioName, Vector3 position, float fadeOutTime = 0f, float fadeInTime = 0f, float fadeInDelay = 0f)
        {
            PlayMusic(audioName, position, transform, fadeOutTime, fadeInTime, fadeInDelay);
        }

        private void PlayMusic(string audioName, Vector3 position, Transform parent, float fadeOutTime = 0f, float fadeInTime = 0f, float fadeInDelay = 0f)
        {
            Music m = GetMusic(audioName);
            if (m != null)
            {
                if (activeMusicObject != null)
                {
                    if (fadeOutTime > 0)
                    {
                        FadeOutMusic(fadeOutTime);
                    }
                    else
                    {
                        audioPool.Release(activeMusicObject);
                    }
                }

                activeMusicObject = audioPool.Get();

                activeMusicObject.transform.position = position;
                activeMusicObject.transform.parent = parent;
                PlayMusic(m, activeMusicObject.GetComponent<AudioSource>());
                if (fadeInTime > 0f)
                {
                    musicTimers.Add(new MusicTimer(TimerType.FadeIn, fadeInTime, activeMusicObject, fadeInDelay));
                }
            }
        }

        // index is specifically for the editor, so that it can choose the segment to play
        private (AudioSegment, float) GetAudioGroupSegment(string audioName, int index = -1)
        {
            AudioGroup audioGroup = audioGroupDict[audioName];
            if (audioGroup == null)
            {
                Debug.LogWarning("No Audio Group with the name: " + audioName);
                return (null, 0f);
            }

            List<AudioGroupSegment> audioGroupSegments = audioGroup.GetSegments();
            if (audioGroupSegments.Count == 0)
            {
                Debug.LogWarning("Audio Group Has no segments to play");
                return (null, 0f);
            }

            float audioGroupVolume = audioGroup.GetVolume();
            AudioGroupType audioGroupType = audioGroup.GetAudioGroupType();

            if (index != -1)
            {
                return (audioGroupSegments[index].GetAudioSegment(), audioGroupVolume);
            }
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

                return (audioGroupSegments[weightedRandomIndex].GetAudioSegment(), audioGroupVolume);
            }
            else // Sequence
            {
                float audioGroupResetTime = audioGroup.GetResetTime();
                if (audioGroupResetTime > 0)
                    sequenceResetTimer[audioGroup] = audioGroupResetTime;

                return (audioGroup.SequenceStep().GetAudioSegment(), audioGroup.GetVolume());
            }
        }

        private AudioSegment GetAudioSegment(string audioName)
        {
            return audioSegmentDict[audioName].GetAudioSegment();
        }

        private Music GetMusic(string musicName)
        {
            return musicDict[musicName];
        }

        private void PlayAudioSegment(AudioSegment audioSegment, float volumeModifier, AudioSource audioSource)
        {
            audioSource.clip = audioSegment.GetAudioClip();
            float volume = audioSegment.GetVolume();
            float pitch = audioSegment.GetPitch();
            float randomVolume = audioSegment.GetRandomVolume();
            float randomPitch = audioSegment.GetRandomPitch();
            float finalVolume = Random.Range(volume - randomVolume, volume + randomVolume) * volumeModifier;
            float finalPitch = Random.Range(pitch - randomPitch, pitch + randomPitch);
            audioSource.volume = finalVolume;
            audioSource.pitch = finalPitch;
            audioSource.loop = false;
            audioSource.Play();
        }

        private void PlayMusic(Music m, AudioSource audioSource)
        {
            audioSource.clip = m.GetAudioClip();
            audioSource.volume = m.GetVolume();
            audioSource.pitch = 1f;
            audioSource.loop = true;
            audioSource.Play();
        }

        private void BuildAudioDicts()
        {
            audioGroupDict.Clear();
            foreach (AudioGroup audioGroup in audioGroups)
                audioGroupDict.Add(audioGroup.GetName(), audioGroup);

            audioSegmentDict.Clear();
            foreach (NamedAudioSegment audioSegment in audioSegments)
                audioSegmentDict.Add(audioSegment.GetName(), audioSegment);

            musicDict.Clear();
            foreach (Music m in music)
                musicDict.Add(m.GetName(), m);
        }

        #region PoolFunctions
        private static GameObject OnCreateAudio()
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
}
