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

#if UNITY_EDITOR && UNITY_2019_3_OR_NEWER
            // When domain reload is disabled, re-initialization is required when entering play mode; 
            // otherwise, pending tasks will leak between play mode sessions.
            var domainReloadDisabled = UnityEditor.EditorSettings.enterPlayModeOptionsEnabled &&
                                       UnityEditor.EditorSettings.enterPlayModeOptions.HasFlag(UnityEditor.EnterPlayModeOptions.DisableDomainReload);
            if (!domainReloadDisabled && Runners.Length > 0) return;
#else
            if (Runners.Length > 0) return; // already initialized
#endif
            
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