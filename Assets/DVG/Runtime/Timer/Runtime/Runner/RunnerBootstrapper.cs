using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace DVG.Timers
{
    internal static class RunnerBootstrapper
    {
		private static void SetupRunners()
		{
			TimerUpdater.EarlyUpdate = new TimerRunner(TimerManager.EarlyUpdateStorage);
			TimerUpdater.FixedUpdate = new TimerRunner(TimerManager.FixedUpdateStorage);
			TimerUpdater.UpdateRunner = new TimerRunner(TimerManager.UpdateStorage);
			TimerUpdater.PreLateUpdate = new TimerRunner(TimerManager.PreLateUpdateStorage);
			TimerUpdater.PostLateUpdate = new TimerRunner(TimerManager.PostLateUpdateStorage);
		}

		internal static void Initialize()
		{
			SetupRunners();
			
			var timerEarlyUpdate = new PlayerLoopSystem
			{
				type = typeof(TimerEarlyUpdate),
				updateDelegate = static () => TimerUpdater.EarlyUpdate.Update(Time.deltaTime, Time.unscaledDeltaTime)
			};
			var timerFixedUpdate = new PlayerLoopSystem
			{
				type = typeof(TimerFixedUpdate),
				updateDelegate = static () => TimerUpdater.FixedUpdate.Update(Time.fixedDeltaTime, Time.fixedUnscaledDeltaTime)
			};
			var timerUpdate = new PlayerLoopSystem
			{
				type = typeof(TimerUpdate),
				updateDelegate = static () => TimerUpdater.UpdateRunner.Update(Time.deltaTime, Time.unscaledDeltaTime)
			};
			var timerPreLateUpdate = new PlayerLoopSystem
			{
				type = typeof(TimerPreLateUpdate),
				updateDelegate = static () => TimerUpdater.PreLateUpdate.Update(Time.deltaTime, Time.unscaledDeltaTime)
			};
			var timerPostLateUpdate = new PlayerLoopSystem
			{
				type = typeof(TimerPostLateUpdate),
				updateDelegate = static () => TimerUpdater.PostLateUpdate.Update(Time.deltaTime, Time.unscaledDeltaTime)
			};

			PlayerLoopSystem currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();

			InsertSystems(ref currentPlayerLoop,
                earlyUpdateLoop: timerEarlyUpdate,
                fixedUpdateLoop: timerFixedUpdate,
                updateLoop: timerUpdate,
                preLateUpdateLoop: timerPreLateUpdate,
                postLateUpdateLoop: timerPostLateUpdate
            );

            PlayerLoop.SetPlayerLoop(currentPlayerLoop);
            
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += OnPlaymodeExit;
#endif
        }

		private static void InsertSystems(ref PlayerLoopSystem rootSystem,
            in PlayerLoopSystem earlyUpdateLoop,
            in PlayerLoopSystem fixedUpdateLoop,
            in PlayerLoopSystem updateLoop,
            in PlayerLoopSystem preLateUpdateLoop,
            in PlayerLoopSystem postLateUpdateLoop)
        {
            const int insertIndex = 0;
            
            Span<PlayerLoopSystem> rootSpan = rootSystem.subSystemList;

            for (int i = 0; i < rootSpan.Length; i++)
            {
                ref PlayerLoopSystem subSystem = ref rootSpan[i];

                PlayerLoopSystem insertSystem;

                Type type = subSystem.type;

                if (type == typeof(EarlyUpdate)) insertSystem = earlyUpdateLoop;
                else if (type == typeof(FixedUpdate)) insertSystem = fixedUpdateLoop;
                else if (type == typeof(Update)) insertSystem = updateLoop;
                else if (type == typeof(PreLateUpdate)) insertSystem = preLateUpdateLoop;
                else if (type == typeof(PostLateUpdate)) insertSystem = postLateUpdateLoop;
                else continue;

                PlayerLoopSystem[] oldList = subSystem.subSystemList;

                int oldLength = oldList?.Length ?? 0;

                PlayerLoopSystem[] newList = new PlayerLoopSystem[oldLength + 1];

                newList[insertIndex] = insertSystem;

                if (oldLength > 0) Array.Copy(oldList, 0, newList, insertIndex + 1, oldLength);

                subSystem.subSystemList = newList;
            }
        }

#if UNITY_EDITOR
        private static void OnPlaymodeExit(PlayModeStateChange playModeState)
        {
            if (playModeState == PlayModeStateChange.ExitingPlayMode)
            {
				TimerUpdater.EarlyUpdate.Dispose();
				TimerUpdater.FixedUpdate.Dispose();
				TimerUpdater.UpdateRunner.Dispose();
				TimerUpdater.PreLateUpdate.Dispose();
				TimerUpdater.PostLateUpdate.Dispose();

                var currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
                
                currentPlayerLoop.RemoveSystemFrom<EarlyUpdate>(typeof(TimerEarlyUpdate));
                currentPlayerLoop.RemoveSystemFrom<FixedUpdate>(typeof(TimerFixedUpdate));
                currentPlayerLoop.RemoveSystemFrom<Update>(typeof(TimerUpdate));
                currentPlayerLoop.RemoveSystemFrom<PreLateUpdate>(typeof(TimerPreLateUpdate));
                currentPlayerLoop.RemoveSystemFrom<PostLateUpdate>(typeof(TimerPostLateUpdate));
                
                PlayerLoop.SetPlayerLoop(currentPlayerLoop);

                EditorApplication.playModeStateChanged -= OnPlaymodeExit;
            }
        }
        
        private static void RemoveSystemFrom<TLoop>(this ref PlayerLoopSystem rootSystem, Type systemToRemove)
        {
            PlayerLoopSystem[] rootSubSystems = rootSystem.subSystemList;

            if (rootSubSystems != null)
            {
                int subSystemCount = rootSubSystems.Length;
                List<PlayerLoopSystem> newSubSystemList = new(subSystemCount);

                Span<PlayerLoopSystem> allSubSystemsSpan = rootSubSystems.AsSpan();

                for (int i = 0; i < subSystemCount; ++i)
                {
                    PlayerLoopSystem subSystem = allSubSystemsSpan[i];

                    if (subSystem.type == typeof(TLoop))
                    {
                        subSystem.subSystemList = subSystem.subSystemList?
                            .Where(loopSystem => loopSystem.type != systemToRemove)
                            .ToArray();
                    }

                    newSubSystemList.Add(subSystem);
                }

                rootSystem.subSystemList = newSubSystemList.ToArray();
            }
        }
#endif

		private struct TimerEarlyUpdate{}
		private struct TimerFixedUpdate{}
		private struct TimerUpdate{}
		private struct TimerPreLateUpdate{}
		private struct TimerPostLateUpdate{}
	}
}