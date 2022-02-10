using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using JauntyBear.UnityData;

namespace JauntyBear.UnityAudioEvents
{
    public class MusicPlayer : MonoBehaviour
    {
        const int NB_MUSIC_AUDIOSOURCES = 2;
        const float MUSIC_VOLUME = 1f;

        [Header("Bindings")]
        [SerializeField] BoolVariable _enableMusic;

        [Header("Settings")]
        [SerializeField] AudioSource[] _musicSources;
        [SerializeField] AudioClip _audioClip;
        [SerializeField] bool _playOnEnable = false;
        [Min(0f)]
        [SerializeField] float _musicFadeDuration = 2.5f;
        [Min(0f)]
        [SerializeField] private int volumeChangesPerSecond = 10;
        [SerializeField] bool _randomStartingPosition = false;

        #region public properties
        public bool IsMusicEnabled => _enableMusic.Value;
        public bool IsMusicFading
        {
            get
            {
                foreach (IEnumerator i in _musicFaders)
                {
                    if (i != null)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        public int ActiveMusicSource => _activeMusicSourceIndex;
        public int NextMusicSource => _activeMusicSourceIndex == 0 ? 1 : 0;
        public string ActiveClip => _musicSources[_activeMusicSourceIndex].clip ? _musicSources[_activeMusicSourceIndex].clip.name : "#";

        #endregion
        private int _activeMusicSourceIndex = 0;
        private IEnumerator[] _musicFaders = new IEnumerator[NB_MUSIC_AUDIOSOURCES];
        
        #region unity callbacks
        void Awake()
        {
            if (_musicSources.Length != NB_MUSIC_AUDIOSOURCES)
            {
                Debug.LogErrorFormat("MusicPlayer requires {0} audiosources", NB_MUSIC_AUDIOSOURCES);
                return;
            }
            foreach (AudioSource audioSource in _musicSources)
            {
                audioSource.playOnAwake = false;
            }
        }

        void OnEnable()
        {
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            _enableMusic.VariableChange += EnableMusicPlauer;
            if (_playOnEnable && IsMusicEnabled)
                Play();
        }

        private void Start()
        {
            if (_enableMusic == null)
                Debug.LogError("BoolVariable _enableMusic required");
        }

        void OnDisable()
        {
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
            _enableMusic.VariableChange -= EnableMusicPlauer;
        }

        private void OnSceneUnloaded(Scene current)
        {
            Stop();
        }
        #endregion

        public void EnableMusicPlauer(bool enableMusicPlayer)
        {
            if (enableMusicPlayer)
                Play();
            else
                Stop();
        }

        public void Play()
        {
            if (!IsMusicEnabled)
            {
                return;
            }
            else if (_audioClip != null)
            {
                StartCoroutine(PlayMusicCo(_audioClip));
            }
            else
            {
                Debug.LogWarning("MusicPlayer does not contain any audioClip.");
            }
        }

        public void LoadAndPlay(string audioClipKey)
        {
            StartCoroutine(LoadAndPlayCo(audioClipKey));
        }

        private IEnumerator LoadAndPlayCo(string audioClipKey)
        {
            yield return StartCoroutine(LoadMusicCo(audioClipKey));
            if (!_enableMusic.Value)
            {
                yield return null; // music disabled, exit
            }
            else if (_audioClip != null)
            {
                StartCoroutine(PlayMusicCo(_audioClip));
            }
            else
            {
                Debug.LogWarning("LoadAndPlayCo from MusicPlayer does not get any audioClip.");
                yield return null;
            }
        }

        private IEnumerator PlayMusicCo(AudioClip audioClip)
        {
            if (!IsMusicEnabled)
            {
                yield break; // music disabled, exit
            }
            //Prevent fading the same clip on both players 
            if (audioClip == _musicSources[_activeMusicSourceIndex].clip && _musicSources[_activeMusicSourceIndex].volume > 0)
            {
                yield break;
            }
            //Kill all playing
            foreach (IEnumerator i in _musicFaders)
            {
                if (i != null)
                {
                    StopCoroutine(i);
                }
            }
            //Fade-out the active play, if it is not silent (eg: first start)
            if (_musicSources[_activeMusicSourceIndex].volume > 0)
            {
                _musicFaders[0] = FadeAudioSource(audioSource: _musicSources[_activeMusicSourceIndex]
                    , duration: _musicFadeDuration
                    , targetVolume: 0.0f
                    , finishedCallback: () => { _musicFaders[0] = null; });
                StartCoroutine(_musicFaders[0]);
            }

            //Fade-in the new clip
            int nextMusicSource = (_activeMusicSourceIndex + 1) % _musicSources.Length;
            _musicSources[nextMusicSource].clip = audioClip;
            if (_randomStartingPosition)
            {
                _musicSources[nextMusicSource].time = audioClip.length * Random.Range(0f, 1f);
            }
            _musicSources[nextMusicSource].Play();
            _musicFaders[1] = FadeAudioSource(audioSource: _musicSources[nextMusicSource]
                , duration: _musicFadeDuration
                , targetVolume: MUSIC_VOLUME
                , finishedCallback: () => { _musicFaders[1] = null; });
            StartCoroutine(_musicFaders[1]);

            //Register new active player
            _activeMusicSourceIndex = nextMusicSource;
        }

        public void Stop()
        {
            if (!IsMusicEnabled)
            {
                _musicSources[ActiveMusicSource].Stop();
                _musicSources[NextMusicSource].Stop();
                return;
            }

            if (_musicSources[_activeMusicSourceIndex].isPlaying)
            {
                if (_musicSources[_activeMusicSourceIndex].volume > 0)
                {
                    _musicFaders[0] = FadeAudioSource(audioSource: _musicSources[_activeMusicSourceIndex]
                        , duration: _musicFadeDuration * 0.3f
                        , targetVolume: 0.0f
                        , finishedCallback: () => { _musicFaders[0] = null; });
                    StartCoroutine(_musicFaders[0]);
                }
            }
        }

        public void ResumeMusic()
        {
            if (!_enableMusic.Value) return; // music disabled
            _musicSources[_activeMusicSourceIndex].Play();
        }

        /// <summary>
        /// Fades an AudioSource(player) during a given amount of time(duration) to a specific volume(targetVolume)
        /// </summary>
        /// <param name="audioSource">AudioSource to be modified</param>
        /// <param name="duration">Duration of the fading</param>
        /// <param name="targetVolume">Target volume, the player is faded to</param>
        /// <param name="finishedCallback">Called when finshed</param>
        /// <returns></returns>
        IEnumerator FadeAudioSource(AudioSource audioSource, float duration, float targetVolume, System.Action finishedCallback)
        {
            //Calculate the steps
            int Steps = (int)(volumeChangesPerSecond * duration);
            float StepTime = duration / Steps;
            float StepSize = (targetVolume - audioSource.volume) / Steps;

            //Fade now
            for (int i = 1; i < Steps; i++)
            {
                audioSource.volume += StepSize;
                yield return new WaitForSecondsRealtime(StepTime);
            }
            //Make sure the targetVolume is set
            audioSource.volume = targetVolume;

            //Callback
            if (finishedCallback != null)
            {
                finishedCallback();
            }
        }

        public void LoadMusic(string audioClipKey)
        {
            StartCoroutine(LoadMusicCo(audioClipKey));
        }

        private IEnumerator LoadMusicCo(string audioClipKey)
        {
            AsyncOperationHandle<AudioClip> goHandle = Addressables.LoadAssetAsync<AudioClip>(audioClipKey);
            yield return goHandle;
            if (goHandle.Status == AsyncOperationStatus.Succeeded)
            {
                _audioClip = goHandle.Result;
            }
            else
            {
                Debug.LogWarningFormat("LoadMusicCo cannot not addressable {0}", audioClipKey);
            }
        }
    }
}