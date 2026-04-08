using System;
using UnityEngine;
using UnityEngine.Audio;

namespace DVG.Audio
{
    [Serializable]
    public sealed class BackgroundController : IAudioController
    {
        [SerializeField] private AudioMixerGroup _backgroundMixerGroup;
        [SerializeField] private AudioEmitter _backgroundEmitter;

        public AudioEmitter Play(IAudioData audioData)
        {
            _backgroundEmitter
                .WithAudioClip(audioData.AudioClip)
                .WithLoop(true)
                .Play();

            return _backgroundEmitter;
        }

        public void Stop(IAudioData audioData)
        {
            throw new NotImplementedException();
        }

        public void StopAll()
        {
            throw new NotImplementedException();
        }
        
        public bool CanPlay(IAudioData audioData)
        {
            throw new NotImplementedException();
        }
    }
}