using UnityEngine;

namespace DVG.StateMachine
{
    public abstract class StateMachine<TOwner> : IStateMachine 
        where TOwner : MonoBehaviour
    {
        [SerializeField] protected TOwner Owner;
        
        protected State<TOwner> _currentState;
        protected State<TOwner> _previousState;

        public void ChangeState(State state)
        {
            _currentState?.OnExit(Owner);
            _previousState = _currentState;
            _currentState = state;
            _currentState?.OnEnter(Owner);
        }

        public void ClearState()
        {
            _currentState?.OnExit(Owner);
            _previousState = _currentState = null;
        }
    }
}
