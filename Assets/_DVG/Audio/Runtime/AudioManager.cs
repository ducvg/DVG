using System;
using System.Collections.Generic;
using UnityEngine;

namespace DVG.Audio
{
    public sealed class AudioManager : PersistentSingleton<AudioManager>
    {
        [field: SerializeField] public AudioEmitterFactory EmitterFactory {get; private set;} 
        
        [SerializeReference] private IAudioController[] _controllers; 
        private Dictionary<Type, IAudioController> _controllersDict;

        protected override void Awake()
        {
            base.Awake();
            _controllersDict = new(_controllers.Length);
            foreach(var controller in _controllers)
            {
                _controllersDict.Add(controller.GetType(), controller);
            }

            #if !UNITY_EDITOR
                _controllers = null;
            #endif
        }

        public static T Get<T>() where T : IAudioController
        {
            return (T)Instance._controllersDict[typeof(T)];
        }

        #if UNITY_EDITOR
        public static void FetchControllers()
        {
            var availableControllers = UnityEditor.TypeCache.GetTypesDerivedFrom<IAudioController>();

            Instance._controllers = new IAudioController[availableControllers.Count];
            for(int i = 0; i < availableControllers.Count; i++)
            {
                Instance._controllers[i] = (IAudioController)Activator.CreateInstance(availableControllers[i]);
            }
        }
        #endif
    }
}