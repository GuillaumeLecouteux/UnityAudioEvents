using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

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

        public override void Play(AudioSource source, float runtimeVolume)
        {
            if (_audioClips.Count == 0 || (chanceOfPlaying < 1f && Random.value > chanceOfPlaying))
                return;
            _randomIndex = Random.Range(0, _audioClips.Count);
            source.clip = _audioClips[_randomIndex];
            source.volume = _volumeRange.RandomInclusive * runtimeVolume;
            source.pitch = _pitchRange.RandomInclusive;
            source.Play();
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