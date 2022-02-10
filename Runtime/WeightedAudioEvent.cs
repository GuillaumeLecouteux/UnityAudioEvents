using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace JauntyBear.UnityAudioEvents
{
    [CreateAssetMenu(fileName = "AudioEvent", menuName="Audio Events/WeightedAudioEvent")]
    public class WeightedAudioEvent : AAudioEvent
    {
        [Serializable]
        public struct CompositeEntry
        {
            public AAudioEvent audioEvent;
            public float weight;
        }

        [Header("Bindings")]
        [SerializeField] private CompositeEntry[] _weightedAudioEvents;

        private int _randomIndex;

        public override void Play(AudioSource source)
        {
            float totalWeight = 0;
            for (int i = 0; i < _weightedAudioEvents.Length; ++i)
                totalWeight += _weightedAudioEvents[i].weight;

            float pick = Random.Range(0, totalWeight);
            for (int i = 0; i < _weightedAudioEvents.Length; ++i)
            {
                if (pick > _weightedAudioEvents[i].weight)
                {
                    pick -= _weightedAudioEvents[i].weight;
                    continue;
                }
                _randomIndex = i;
                _weightedAudioEvents[i].audioEvent.Play(source);
                return;
            }
        }

        public override void AddAudioClip(AudioClip newAudioClip)
        {
            throw new NotImplementedException();
        }

        public override AudioClip GetAudioClip()
        {
            return _weightedAudioEvents[_randomIndex].audioEvent.GetAudioClip();
        }

        public override float Duration => _weightedAudioEvents.Length > 0 ? _weightedAudioEvents[_randomIndex].audioEvent.Duration : 0f;
    }
}