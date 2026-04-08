using System;
using UnityEngine;

namespace DVG.StateMachine
{
    public interface IState<TOwner> where TOwner : MonoBehaviour
    {
        public void OnEnter(TOwner owner);
        public void OnExit(TOwner owner);
    }
    
    public abstract class StateMachine<TOwner> where TOwner : MonoBehaviour
    {
        [SerializeField] protected TOwner Owner;
        
        protected IState<TOwner> _currentState;
        protected IState<TOwner> _previousState;

        public void ChangeState<TState>(TState newState) where TState : IState<TOwner>
        {
            StateRunner.Deregister(_currentState);
            _currentState?.OnExit(Owner);
            
            _previousState = _currentState;
            _currentState = newState;
            
            _currentState?.OnEnter(Owner);
            StateRunner.Register(_currentState);
        }

        public void ClearState()
        {
            StateRunner.Deregister(_currentState);
            _currentState?.OnExit(Owner);
            
            _previousState = _currentState = null;
        }
    }
}
