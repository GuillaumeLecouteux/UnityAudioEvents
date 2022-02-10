using UnityEngine;
using JauntyBear.UnityData;

namespace JauntyBear.UnityAudioEvents
{
    [CreateAssetMenu(fileName = "AudioEvent", menuName="Audio Events/SingleAudioEvent")]
    public class SingleAudioEvent : AAudioEvent
    {
        [Header("Bindings")]
        [SerializeField] private AudioClip _audioClip;
        [Header("Settings")]
        [SerializeField] private RangeFloat volume = new RangeFloat(1f,1f);
        [SerializeField] private RangeFloat pitch = new RangeFloat(1f,1f);
        [Range(0f, 1f)]
        [SerializeField] private float chanceOfPlaying = 1f;
        public override void Play(AudioSource source)
        {
            if (_audioClip == null || (chanceOfPlaying < 1f && Random.value > chanceOfPlaying))
                return;
            source.clip = _audioClip;
            source.volume = volume.RandomInclusive;
            source.pitch = pitch.RandomInclusive;
            source.Play();
        }

        public override void AddAudioClip(AudioClip audioClip)
        {
            _audioClip = audioClip;
        }

        public override AudioClip GetAudioClip()
        {
            return _audioClip;
        }

        public override float Duration => _audioClip.length;
    }
}