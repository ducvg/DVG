using System;
using System.Collections.Generic;
using UnityEngine;

namespace DVG.Audio
{
    public sealed class AudioManager : MonoBehaviour
    {
        [field: SerializeField] public AudioEmitterFactory EmitterFactory {get; private set;} 
        [SerializeReference] private IAudioController[] _controllers; 
        private Dictionary<Type, IAudioController> _controllersDict;

        public static AudioManager Instance {get; private set;} 

        void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            EmitterFactory.Init();

            TransferControllerDict();
        }

        private void TransferControllerDict()
        {
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
    }

}