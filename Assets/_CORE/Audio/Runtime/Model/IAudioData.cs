using UnityEngine;

namespace DVG.Audio
{
    public interface IAudioData
    {
        public int MaxInstances { get; } 
        public AudioClip AudioClip { get; } 
    }
}