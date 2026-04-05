using System;
using UnityEngine;

namespace DVG.Audio
{

    [Serializable]
    public class SingleAudioData : IAudioData
    {
        [field: SerializeField] public int MaxInstances { get; private set; } = 1;
        [field: SerializeField] public AudioClip AudioClip { get; private set; }
    }
}