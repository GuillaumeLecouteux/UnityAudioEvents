using UnityEngine;

namespace JauntyBear.UnityAudioEvents
{
    public class AudioEventPlayer : MonoBehaviour
    {
		private const string DEFAULT_AUDIOSOURCE_TAG = "DefaultAudioSource";
        [SerializeField] protected AudioSource _audioSource;
        [SerializeField] protected AAudioEvent _audioEvent;
        [SerializeField] bool _playOnEnable = false;

        void Awake()
        {
            if (_audioSource == null)
            {
                _audioSource = GetComponent<AudioSource>();
                if (_audioSource == null)
                {
                    GameObject defaultAudioSource = GameObject.FindWithTag(DEFAULT_AUDIOSOURCE_TAG);
					if(defaultAudioSource != null)
						_audioSource = defaultAudioSource.GetComponent<AudioSource>();
                }
                if (_audioSource == null)
                {
                    _audioSource = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
                }
            }
        }

        void OnEnable()
        {
            if(_playOnEnable)
                Play();
        }

 	public void SetAudioEvent(AAudioEvent audioEvent)
	{
	    _audioEvent=audioEvent;
	}

        public void Play()
        {
            if (_audioEvent != null && enabled == true)
            {
                _audioEvent.Play(_audioSource);
            }
            else if (_audioEvent == null)
            {
                Debug.LogWarning("AudioEventPlayer does not contain any audioEvent.");
            }
            else
            {
                Debug.LogWarning("AudioEventPlayer is disabled.");
            }
        }

        public void Play(AAudioEvent audioEvent)
        {
            audioEvent.Play(_audioSource);
        }

        public void Play(AudioSource audioSource)
        {
            _audioEvent.Play(audioSource);
        }

        public void Stop()
        {
            _audioSource.Stop();
        }
    }
}
