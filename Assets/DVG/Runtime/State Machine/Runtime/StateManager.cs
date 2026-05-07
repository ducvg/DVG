using System;
using UnityEngine;

namespace DVG.StateMachine
{
    public static class StateManager
    {
        internal static IStateRunner[] Runners { get; }

        static StateManager()
        {
            Runners = new IStateRunner[Enum.GetValues(typeof(UpdateType)).Length];
        }
        
        public static void Register<TOwner>(State<TOwner> state) where TOwner : MonoBehaviour
        {
            foreach (var runner in Runners)
            {
                runner.Register(state);
            }
        }

        public static void Unregister<TOwner>(State<TOwner> state) where TOwner : MonoBehaviour
        {
            foreach (var runner in Runners)
            {
                runner.Unregister(state);
            }
        }
    }
}