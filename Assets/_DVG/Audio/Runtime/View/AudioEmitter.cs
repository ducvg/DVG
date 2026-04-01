using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Audio;

namespace DVG.Audio
{
    public sealed class AudioEmitter : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        private bool _isPaused = false;
        public new Transform transform;

        void Awake()
        {
            transform = base.transform;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Play()
        {
            audioSource.Play();
            _isPaused = false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Stop()
        {
            audioSource.Stop();
            _isPaused = false;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Pause()
        {
            audioSource.Pause();
            _isPaused = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Resume()
        {
            audioSource.UnPause();
            _isPaused = false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsPaused()
        {
            return _isPaused;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsPlaying()
        {            
            bool isPlaying = audioSource.isPlaying;
            return isPlaying;
        }
        
        #region Config Builder
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public AudioEmitter WithPosition(Vector3 position)
            {
                transform.position = position;
                return this;
            }
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public AudioEmitter WithParent(Transform parent)
            {
                transform.SetParent(parent);
                return this;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public AudioEmitter WithAudioClip(AudioClip audioClip)
            {
                audioSource.clip = audioClip;
                return this;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public AudioEmitter WithMixerGroup(AudioMixerGroup mixerGroup)
            {
                audioSource.outputAudioMixerGroup = mixerGroup;
                return this;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public AudioEmitter WithLoop(bool isLoop)
            {
                audioSource.loop = isLoop;
                return this;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public AudioEmitter WithVolume(float volume)
            {
                audioSource.volume = volume;
                return this;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public AudioEmitter WithMute(bool isMute)
            {
                audioSource.mute = isMute;
                return this;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public AudioEmitter WithPitch(float pitch)
            {
                audioSource.pitch = pitch;
                return this;
            }
        #endregion
    }
}