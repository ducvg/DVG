using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace DVG.StateMachine
{
    public static class RunnerBootstrapper
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        internal static void Initialize() 
        {
            PlayerLoopSystem currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();

            PlayerLoopSystem stateUpdateLoop = new()
            {
                type = typeof(StateRunner),
                updateDelegate = StateRunner.EarlyUpdate            
            };

            InsertSystemIn<Update>(ref currentPlayerLoop, stateUpdateLoop, insertIndex: 0);
            PlayerLoop.SetPlayerLoop(currentPlayerLoop);
        }

        private static void InsertSystemIn<TLoop>(ref PlayerLoopSystem rootSystem, in PlayerLoopSystem newSystem, int insertIndex)
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