using System;
using UnityEngine;
using DVG.Pool;

namespace DVG.Audio
{
    [Serializable]
    public sealed class AudioEmitterFactory
    {
        [SerializeField] private AudioEmitter _audioEmitterPrefab;
        private const int c_defaultCapacity = 64;
        private const int c_maxSize = 512; // == max virtual count in project setting
        
        private readonly UnityPool<AudioEmitter> _emitterPool; // always leave active

        public AudioEmitterFactory()
        {
            _emitterPool = new (_audioEmitterPrefab, c_defaultCapacity, c_maxSize);
        }

        public AudioEmitter CreateEmitter()
        {
            return _emitterPool.Rent();
        }

        public void ReturnEmitter(AudioEmitter emitter)
        {
            if(!emitter) return;

            _emitterPool.Return(emitter);
        }
    }
}