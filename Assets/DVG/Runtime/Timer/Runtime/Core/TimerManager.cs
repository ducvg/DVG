using UnityEditor;
using UnityEngine;

namespace DVG.Timers
{
    internal static class TimerManager
    {
        private const int Storage_Initial_Capacity = 256;

        internal static TimerDataStorage EarlyUpdateStorage;
        internal static TimerDataStorage FixedUpdateStorage;
        internal static TimerDataStorage UpdateStorage;
        internal static TimerDataStorage PreLateUpdateStorage;
        internal static TimerDataStorage PostLateUpdateStorage;

		private static bool initialized = false;
#if UNITY_2020_1_OR_NEWER
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
#else
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
#endif        
        private static void Initialize()
        {
#if UNITY_EDITOR
            var domainReloadDisabled = EditorSettings.enterPlayModeOptionsEnabled && EditorSettings.enterPlayModeOptions.HasFlag(EnterPlayModeOptions.DisableDomainReload);
            if (!domainReloadDisabled && initialized) return;
#else
            if (initialized) return;
#endif
			if(!initialized)
			{
				initialized = true;
				EarlyUpdateStorage = new TimerDataStorage(Storage_Initial_Capacity);
            	FixedUpdateStorage = new TimerDataStorage(Storage_Initial_Capacity);
            	UpdateStorage = new TimerDataStorage(Storage_Initial_Capacity);
            	PreLateUpdateStorage = new TimerDataStorage(Storage_Initial_Capacity);
            	PostLateUpdateStorage = new TimerDataStorage(Storage_Initial_Capacity);
			}
			RunnerBootstrapper.Initialize();
        }
    }
}