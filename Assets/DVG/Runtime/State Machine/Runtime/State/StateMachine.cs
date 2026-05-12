using System.Threading;
using UnityEngine;

namespace DVG.StateMachine
{
    public abstract class StateMachine<TOwner> where TOwner : MonoBehaviour
    {
        protected TOwner Owner;
        
        protected State<TOwner> _currentState;

        private CancellationTokenRegistration _destroyCtRegistration;

        public void SetOwner(TOwner owner)
        {
            if(Owner == owner) return;
            Owner = owner;
            //incase forgot stateMachine.Dispose()
            //must manually call Dispose() if not set owner
            _destroyCtRegistration = Owner.destroyCancellationToken.Register(Dispose); 
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
            
            _currentState = null;
        }

        public virtual void Dispose() //must manully call this if not set onwer
        {
            ClearState();
            _destroyCtRegistration.Dispose();
        }
    }
}
