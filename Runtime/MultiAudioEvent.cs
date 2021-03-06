using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using JauntyBear.UnityData;

namespace JauntyBear.UnityAudioEvents
{
    [CreateAssetMenu(fileName = "AudioEvent", menuName="Audio Events/MultiClipsAudioEvent")]
    public class MultiAudioEvent : AAudioEvent
    {
        [Header("Bindings")]
        [SerializeField] private List<AudioClip> _audioClips;
        [Header("Settings")]
        [SerializeField] private RangeFloat _volumeRange = new RangeFloat(1f,1f);
        [SerializeField] private RangeFloat _pitchRange = new RangeFloat(1f,1f);
        [Range(0f,1f)]
        [SerializeField] private float chanceOfPlaying = 1f;

        private int _randomIndex;

        public override void Play(AudioSource audioSource)
        {
            if (_audioClips.Count == 0 || (chanceOfPlaying < 1f && Random.value > chanceOfPlaying))
                return;
            _randomIndex = Random.Range(0, _audioClips.Count);
            audioSource.clip = _audioClips[_randomIndex];
            audioSource.volume = _volumeRange.RandomInclusive;
            audioSource.pitch = _pitchRange.RandomInclusive;
            audioSource.Play();
        }

        public override void AddAudioClip(AudioClip newAudioClip)
        {
            _audioClips.Add(newAudioClip);
        }

        public override AudioClip GetAudioClip()
        {
            return _audioClips.Count > 0 ? _audioClips[_randomIndex] : null;
        }

        public override float Duration => _audioClips.Count>0?_audioClips[_randomIndex].length:0f;
    }
}