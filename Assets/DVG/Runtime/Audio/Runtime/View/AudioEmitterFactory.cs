using System;
using UnityEngine;
using DVG.Pool;

namespace DVG.Audio
{
    [Serializable]
    public sealed class AudioEmitterFactory
    {
        [SerializeField] private AudioEmitter _audioEmitterPrefab;
        [SerializeField] private int _defaultCapacity = 64;
        [SerializeField] private int _maxSize = 512; //check max virtual audio in project setting
        
        private UnityPool<AudioEmitter> _emitterPool; //no need disable,enable

        public void Init()
        {
            _emitterPool = new
            (
                _audioEmitterPrefab,
                _defaultCapacity, 
                _maxSize
            );
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