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

            PlayerLoopSystem earlyUpdateLoop = CreateStateRunnerLoop(StateRunner.EarlyUpdate);
            PlayerLoopSystem updateLoop = CreateStateRunnerLoop(StateRunner.Update);
            PlayerLoopSystem preLateUpdateLoop = CreateStateRunnerLoop(StateRunner.PreLateUpdate);
            PlayerLoopSystem fixedUpdate = CreateStateRunnerLoop(StateRunner.FixedUpdate);

            currentPlayerLoop.InsertSystemAt<EarlyUpdate>(earlyUpdateLoop, insertIndex: 0);
            currentPlayerLoop.InsertSystemAt<Update>(updateLoop, insertIndex: 0);
            currentPlayerLoop.InsertSystemAt<PreLateUpdate>(preLateUpdateLoop, insertIndex: 0); //lateupdate() is inside PreLateUpdate, this run before it
            currentPlayerLoop.InsertSystemAt<FixedUpdate>(fixedUpdate, insertIndex: 0);
            
            PlayerLoop.SetPlayerLoop(currentPlayerLoop);
        }

        private static PlayerLoopSystem CreateStateRunnerLoop(PlayerLoopSystem.UpdateFunction updateDelegate)
        {
            return new PlayerLoopSystem
            {
                type = typeof(StateRunner),
                updateDelegate = updateDelegate
            };
        }

        private static void InsertSystemAt<TLoop>(this ref PlayerLoopSystem rootSystem, in PlayerLoopSystem newSystem, int insertIndex)
        {
            PlayerLoopSystem[] rootSubSystems = rootSystem.subSystemList;

            if (rootSubSystems != null)
            {
                int subSystemCount = rootSubSystems.Length;
                List<PlayerLoopSystem> newSubSystemList = new(subSystemCount + 4);

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