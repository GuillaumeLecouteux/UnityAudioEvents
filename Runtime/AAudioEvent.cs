using System.Collections;
using UnityEngine;

namespace JauntyBear.UnityAudioEvents
{
    public abstract class AAudioEvent : ScriptableObject
    {
        public abstract void Play(AudioSource source, float runtimeVolume);
        public abstract void Play(AudioSource source);
        public abstract void AddAudioClip(AudioClip newAudioClip);
        public abstract float Duration { get; }

        public virtual void Play(AudioSource source) => Play(source: source, runtimeVolume: 1f);

        public IEnumerator PlayCo(AudioSource source)
        {
            Play(source);
            yield return new WaitForSeconds(Duration);
        }

        public abstract AudioClip GetAudioClip();

        public virtual void Stop(AudioSource source)
        {
            source.Stop();
        }
    }
}