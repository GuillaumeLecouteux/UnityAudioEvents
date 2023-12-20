using System.Collections;
using UnityEngine;
using JauntyBear.UnityData;

namespace JauntyBear.UnityAudioEvents
{
    [CreateAssetMenu(fileName = "AudioEvent", menuName= "Audio Events/PitchedAudioEvent")]
    public class PitchedAudioEvent : AAudioEvent
    {
        [Header("Bindings")]
        [SerializeField] private AudioClip _audioClip;
        [Header("Settings")]
        [SerializeField] private RangeFloat _volumeRange = new RangeFloat(1f,1f);
        [Min(0f)]
        [SerializeField] private float _pitch = 1f;
        [Range(0f, 1f)]
        [SerializeField] private float chanceOfPlaying = 1f;

        private AudioSource _currentAudiosource;

        public void SetPitch(float pitch)
        {
            if(_currentAudiosource)
                _currentAudiosource.pitch = pitch * _pitch;
        }

        public override void Play(AudioSource source, float runtimeVolume)
        {
            if (_audioClip == null || (chanceOfPlaying < 1f && Random.value > chanceOfPlaying))
                return;
            _currentAudiosource = source;
            source.clip = _audioClip;
            source.volume = _volumeRange.RandomInclusive * runtimeVolume;
            source.pitch = _pitch;
            source.Play();
        }

        public override void AddAudioClip(AudioClip newAudioClip)
        {
            _audioClip = newAudioClip;
        }

        public override AudioClip GetAudioClip()
        {
            return _audioClip;
        }

        public override float Duration => _audioClip.length;
    }
}