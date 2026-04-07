using UnityEngine;

namespace DVG.StateMachine
{
    public abstract class State<TStateMachine> where TStateMachine : IStateMachine
    {
        public abstract void OnEnter(TStateMachine owner);
        public abstract void OnExit(TStateMachine owner);
    }
}