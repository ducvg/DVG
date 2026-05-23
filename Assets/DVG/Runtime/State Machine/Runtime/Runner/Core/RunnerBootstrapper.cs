using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace DVG.StateMachine
{
    internal static class RunnerBootstrapper
    {
        internal static void Initialize() 
        {
            PlayerLoopSystem currentPlayerLoop = 
#if UNITY_2019_3_OR_NEWER
                PlayerLoop.GetCurrentPlayerLoop();
#else
                PlayerLoop.GetDefaultPlayerLoop();
#endif
            var earlyUpdateRunner = new StateRunnerEarlyUpdate();
            var fixedUpdateRunner = new StateRunnerFixedUpdate();
            var updateRunner = new StateRunnerUpdate();
            var preLateUpdateRunner = new StateRunnerPreLateUpdate();
            var postLateUpdateRunner = new StateRunnerPostLateUpdate();

            PlayerLoopSystem earlyUpdateLoop = new()
            {
                type = typeof(StateRunnerEarlyUpdate),
                updateDelegate = () => earlyUpdateRunner.Run(Time.deltaTime)
            };
            PlayerLoopSystem fixedUpdateLoop = new()
            {
                type = typeof(StateRunnerFixedUpdate),
                updateDelegate = () => fixedUpdateRunner.Run(Time.fixedDeltaTime)
            };
            PlayerLoopSystem updateLoop = new()
            {
                type = typeof(StateRunnerUpdate),
                updateDelegate = () => updateRunner.Run(Time.deltaTime)
            };
            PlayerLoopSystem preLateUpdateLoop = new()
            {
                type = typeof(StateRunnerPreLateUpdate),
                updateDelegate = () => preLateUpdateRunner.Run(Time.deltaTime)
            };
            PlayerLoopSystem postLateUpdateLoop = new()
            {
                type = typeof(StateRunnerPostLateUpdate),
                updateDelegate = () => postLateUpdateRunner.Run(Time.deltaTime)
            };

            StateManager.SetRunners(earlyUpdateRunner, fixedUpdateRunner, updateRunner, preLateUpdateRunner, postLateUpdateRunner);

            InsertSystems(ref currentPlayerLoop,
                earlyUpdateLoop: earlyUpdateLoop,
                fixedUpdateLoop: fixedUpdateLoop,
                updateLoop: updateLoop,
                preLateUpdateLoop: preLateUpdateLoop,
                postLateUpdateLoop: postLateUpdateLoop
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
                var currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
                
                currentPlayerLoop.RemoveSystemFrom<EarlyUpdate>(typeof(StateRunnerEarlyUpdate));
                currentPlayerLoop.RemoveSystemFrom<FixedUpdate>(typeof(StateRunnerFixedUpdate));
                currentPlayerLoop.RemoveSystemFrom<Update>(typeof(StateRunnerUpdate));
                currentPlayerLoop.RemoveSystemFrom<PreLateUpdate>(typeof(StateRunnerPreLateUpdate));
                currentPlayerLoop.RemoveSystemFrom<PostLateUpdate>(typeof(StateRunnerPostLateUpdate));
                
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
                            .Where(s => s.type != systemToRemove)
                            .ToArray();
                    }

                    newSubSystemList.Add(subSystem);
                }

                rootSystem.subSystemList = newSubSystemList.ToArray();
            }
        }
#endif
    }
}