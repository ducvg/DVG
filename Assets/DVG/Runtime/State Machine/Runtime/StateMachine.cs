using UnityEngine;

namespace DVG.StateMachine
{
    public abstract class StateMachine<TOwner> where TOwner : MonoBehaviour
    {
        protected TOwner Owner;
        
        protected IState<TOwner> _currentState;
        protected IState<TOwner> _previousState;

        public void SetOwner(TOwner owner)
        {
            if(Owner == owner) return;
            Owner = owner;
            Owner.destroyCancellationToken.Register(OnDestroy);
        }

        public void ChangeState<TState>(TState newState) where TState : IState<TOwner>
        {
            if(newState == null)
            {
                ClearState();
                return;
            }
            
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

        private void OnDestroy()
        {
            if(_currentState != null) StateRunner.Deregister(_currentState);
        }
    }
}
