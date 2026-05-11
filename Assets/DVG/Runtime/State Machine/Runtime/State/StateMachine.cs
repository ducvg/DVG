using System.Threading;
using UnityEngine;

namespace DVG.StateMachine
{
    public abstract class StateMachine<TOwner> where TOwner : MonoBehaviour
    {
        protected TOwner Owner;
        
        protected State<TOwner> _currentState;
        protected State<TOwner> _previousState;

        private CancellationTokenRegistration _destroyCtRegistration;

        public void SetOwner(TOwner owner)
        {
            if(Owner == owner) return;
            Owner = owner;
            _destroyCtRegistration = Owner.destroyCancellationToken.Register(Destroy); //incase forgot stateMachine.Destroy()
        }

        public void ChangeState<TState>(TState newState) where TState : State<TOwner>
        {
            if(newState == null)
            {
                ClearState();
                return;
            }
            
            if(_currentState != null)
            {
                StateManager.Unregister(_currentState);
                _currentState.OnExit(Owner);
            }
            
            _previousState = _currentState;
            _currentState = newState;
            
            
            _currentState.OnEnter(Owner);
            StateManager.Register(_currentState);
        }

        public void ClearState()
        {
            if (_currentState != null)
            {
                StateManager.Unregister(_currentState);
                _currentState.OnExit(Owner);
            }
            
            _previousState = _currentState = null;
        }

        public virtual void Destroy()
        {
            ClearState();
            _destroyCtRegistration.Dispose();
        }
    }
}
