using System;
using UnityEditor;
using UnityEngine;

namespace DVG.StateMachine
{
    public static class StateManager
    {
        internal static IStateRunner[] Runners { get; private set; }    

#if UNITY_2020_1_OR_NEWER
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
#else
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
#endif        
        public static void Initialize()
        {
            RunnerBootstrapper.Initialize();
        }

        internal static void SetRunners(params IStateRunner[] runners)
        {
            Runners = runners;
        }
        
        public static void Register<TOwner>(State<TOwner> state) where TOwner : MonoBehaviour
        {
            state.IsFinished = false;
            
            foreach (var runner in Runners)
            {
                runner.Register(state);
            }
        }

        public static void Unregister<TOwner>(State<TOwner> state) where TOwner : MonoBehaviour
        {
            state.IsFinished = true;
        }
    }
}