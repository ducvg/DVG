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
            PlayerLoopSystem earlyUpdateLoop = CreateStateRunnerLoop(new StateRunnerEarlyUpdate(), 0);
            PlayerLoopSystem fixedUpdate = CreateStateRunnerLoop(new StateRunnerFixedUpdate(), 1);
            PlayerLoopSystem updateLoop = CreateStateRunnerLoop(new StateRunnerUpdate(),2);
            PlayerLoopSystem preLateUpdateLoop = CreateStateRunnerLoop(new StateRunnerPreLateUpdate(), 3);
            PlayerLoopSystem postLateUpdateLoop = CreateStateRunnerLoop(new StateRunnerPostLateUpdate(), 4);

            currentPlayerLoop.InsertSystemAt<EarlyUpdate>(earlyUpdateLoop, insertIndex: 0);
            currentPlayerLoop.InsertSystemAt<FixedUpdate>(fixedUpdate, insertIndex: 0);
            currentPlayerLoop.InsertSystemAt<Update>(updateLoop, insertIndex: 0);
            currentPlayerLoop.InsertSystemAt<PreLateUpdate>(preLateUpdateLoop, insertIndex: 0); //monobehaviour lateupdate() is inside PreLateUpdate
            currentPlayerLoop.InsertSystemAt<PostLateUpdate>(postLateUpdateLoop, insertIndex: 0);
            
            PlayerLoop.SetPlayerLoop(currentPlayerLoop);
            
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += OnPlaymodeExit;
#endif
        }

        private static PlayerLoopSystem CreateStateRunnerLoop<TStateRunner>(TStateRunner stateRunner, int runnerIndex) where TStateRunner : IStateRunner
        {
            StateManager.Runners[runnerIndex] = stateRunner;
            return new PlayerLoopSystem
            {
                type = typeof(TStateRunner),
                updateDelegate = () => stateRunner.Run(Time.deltaTime, Time.fixedDeltaTime)
            };
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
                        subSystem.subSystemList = subSystem.subSystemList
                            ?.Where(s => s.type != systemToRemove)
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