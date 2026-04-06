using UnityEngine;

namespace DVG.StateMachine
{
    public interface IState<TOwner>
    {
        public void OnEnter(TOwner owner);
        public void OnExit(TOwner owner);
    }
}