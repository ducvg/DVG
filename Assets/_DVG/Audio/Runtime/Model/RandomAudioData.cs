using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace DVG.Audio
{
    [Serializable]
    public class RandomAudioData : IAudioData
    {
        [field: SerializeField] public int MaxInstances { get; private set; } = 1;
        [field: SerializeField] public AudioClip[] AudioClips { get; private set; }
        public AudioClip AudioClip 
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AudioClips[UnityEngine.Random.Range(0, AudioClips.Length)];
        }
    }
}