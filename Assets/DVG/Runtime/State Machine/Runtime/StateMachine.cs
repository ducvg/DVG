using UnityEngine;

namespace DVG.StateMachine
{
    public abstract class StateMachine<TOwner> where TOwner : UnityEngine.Object
    {
        [SerializeField] protected TOwner Owner;
        
        protected IState<TOwner> _currentState;
        protected IState<TOwner> _previousState;

        public void ChangeState<TState>(TState state) where TState : IState<TOwner>
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

        public void Update(float deltaTime)
        {
            _currentState?.OnUpdate(Owner, deltaTime);
        }
    }
}
