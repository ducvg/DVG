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

            currentPlayerLoop.InsertSystemAt<EarlyUpdate>(earlyUpdateLoop, insertIndex: 0);
            currentPlayerLoop.InsertSystemAt<FixedUpdate>(fixedUpdateLoop, insertIndex: 0);
            currentPlayerLoop.InsertSystemAt<Update>(updateLoop, insertIndex: 0);
            currentPlayerLoop.InsertSystemAt<PreLateUpdate>(preLateUpdateLoop, insertIndex: 0); //monobehaviour lateupdate() is inside PreLateUpdate
            currentPlayerLoop.InsertSystemAt<PostLateUpdate>(postLateUpdateLoop, insertIndex: 0);
            
            PlayerLoop.SetPlayerLoop(currentPlayerLoop);
            
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += OnPlaymodeExit;
#endif
        }

        private static void InsertSystemAt<TLoop>(this ref PlayerLoopSystem rootSystem, in PlayerLoopSystem newSystem, int insertIndex)
        {
            PlayerLoopSystem[] rootSubSystems = rootSystem.subSystemList;

            if (rootSubSystems != null)
            {
                int subSystemCount = rootSubSystems.Length;
                List<PlayerLoopSystem> newSubSystemList = new(subSystemCount + 1);

                Span<PlayerLoopSystem> allSubSystemsSpan = rootSubSystems.AsSpan();
                
                for (int i = 0; i < subSystemCount; ++i)
                {
                    PlayerLoopSystem subSystem = allSubSystemsSpan[i];
                    
                    if (subSystem.type == typeof(TLoop))
                    {
                        var tempList = subSystem.subSystemList.ToList();
                        tempList.Insert(insertIndex, newSystem);
                        subSystem.subSystemList = tempList.ToArray();
                    }
                    newSubSystemList.Add(subSystem);
                }

                rootSystem.subSystemList = newSubSystemList.ToArray();
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