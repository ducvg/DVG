using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace DVG.StateMachine
{
    internal static class RunnerBootstrapper
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize() 
        {
            PlayerLoopSystem currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();

            PlayerLoopSystem earlyUpdateLoop = CreateStateRunnerLoop(new StateRunnerEarlyUpdate(), UpdateType.EarlyUpdate);
            PlayerLoopSystem fixedUpdate = CreateStateRunnerLoop(new StateRunnerEarlyUpdate(), UpdateType.FixedUpdate);
            PlayerLoopSystem updateLoop = CreateStateRunnerLoop(new StateRunnerEarlyUpdate(), UpdateType.Update);
            PlayerLoopSystem preLateUpdateLoop = CreateStateRunnerLoop(new StateRunnerEarlyUpdate(), UpdateType.LateUpdate);
            PlayerLoopSystem postLateUpdateLoop = CreateStateRunnerLoop(new StateRunnerEarlyUpdate(), UpdateType.PostLateUpdate);

            currentPlayerLoop.InsertSystemAt<EarlyUpdate>(earlyUpdateLoop, insertIndex: 0);
            currentPlayerLoop.InsertSystemAt<FixedUpdate>(fixedUpdate, insertIndex: 0);
            currentPlayerLoop.InsertSystemAt<Update>(updateLoop, insertIndex: 0);
            currentPlayerLoop.InsertSystemAt<PreLateUpdate>(preLateUpdateLoop, insertIndex: 0); //monobehaviour lateupdate() is inside PreLateUpdate
            currentPlayerLoop.InsertSystemAt<PostLateUpdate>(postLateUpdateLoop, insertIndex: 0);
            
            PlayerLoop.SetPlayerLoop(currentPlayerLoop);
        }

        private static PlayerLoopSystem CreateStateRunnerLoop<TStateRunner>(TStateRunner stateRunner, UpdateType updateType) where TStateRunner : IStateRunner
        {
            StateManager.Runners[(int)updateType] = stateRunner;
            return new PlayerLoopSystem
            {
                type = typeof(TStateRunner),
                updateDelegate = stateRunner.Run
            };
        }

        private static void InsertSystemAt<TLoop>(this ref PlayerLoopSystem rootSystem, in PlayerLoopSystem newSystem, int insertIndex)
        {
            PlayerLoopSystem[] rootSubSystems = rootSystem.subSystemList;

            if (rootSubSystems != null)
            {
                int subSystemCount = rootSubSystems.Length;
                List<PlayerLoopSystem> newSubSystemList = new(subSystemCount + StateManager.UpdateTypeCount);

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
    }
}