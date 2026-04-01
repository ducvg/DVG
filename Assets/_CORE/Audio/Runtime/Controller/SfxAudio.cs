using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

#pragma warning disable CS0649 // unassigned field warning
#pragma warning disable IDE0044 // readonly field suggest

namespace DVG.Audio
{
    [Serializable]
    public sealed class SfxAudio : IAudioController
    {
        [SerializeField] private AudioMixerGroup _sfxMixerGroup;
        private readonly Dictionary<IAudioData, LinkedList<AudioEmitter>> _activeEmittersDict = new(256); //rarely traverse, avoid mem frag 
        
        public AudioEmitter Play(IAudioData audioData)
        {
            if(!CanPlay(audioData)) return null;
            
            AudioEmitter emitter = AudioManager.Instance.EmitterFactory.CreateEmitter();

            emitter
                .WithAudioClip(audioData.AudioClip)
                .WithMixerGroup(_sfxMixerGroup)
                .WithLoop(false)
                .Play();
            
            LinkedListNode<AudioEmitter> emitterNode = _activeEmittersDict[audioData].AddLast(emitter);

            WaitAndReturnEmitter(emitterNode, audioData).Forget();
    
            return emitter;
        }

        private async UniTaskVoid WaitAndReturnEmitter(LinkedListNode<AudioEmitter> emitterNode, IAudioData audioData)
        {
            int clipTimeMs = (int)(audioData.AudioClip.length * 1000);
            await UniTask.Delay(clipTimeMs + 1, cancellationToken: emitterNode.Value.destroyCancellationToken);

            RemoveEmitter(emitterNode, audioData);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanPlay(IAudioData audioData)
        {
            bool hasReachLimit = _activeEmittersDict[audioData].Count >= audioData.MaxInstances;
            return !hasReachLimit;
        }

        public void Stop(IAudioData audioData)
        {
            LinkedListNode<AudioEmitter> currentNode = _activeEmittersDict[audioData].First;
            while(currentNode != null)
            {
                currentNode.Value.Stop();
                RemoveEmitter(currentNode, audioData);
                currentNode = currentNode.Next;
            }
        }

        public void StopAll()
        {
            foreach(LinkedList<AudioEmitter> emitters in _activeEmittersDict.Values)
            {
                foreach(AudioEmitter emitter in emitters)
                {
                    emitter.Stop();
                }
            }

            _activeEmittersDict.Clear();
        }

        // O(n)
        private void RemoveEmitter(AudioEmitter emitter, IAudioData audioData)
        {
            bool isRemoveSucccess = _activeEmittersDict[audioData].Remove(emitter);
            if(!isRemoveSucccess) return;

            AudioManager.Instance.EmitterFactory.ReturnEmitter(emitter);
        }

        // O(1) 
        private void RemoveEmitter(LinkedListNode<AudioEmitter> emitterNode, IAudioData audioData)
        {
            if(emitterNode == null || emitterNode.Value == null) return;

            _activeEmittersDict[audioData].Remove(emitterNode);
            AudioManager.Instance.EmitterFactory.ReturnEmitter(emitterNode.Value);
        }
    }
}