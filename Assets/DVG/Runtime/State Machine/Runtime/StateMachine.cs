using UnityEngine;

namespace DVG.StateMachine
{
    public abstract class StateMachine<TOwner> : IStateMachine 
        where TOwner : UnityEngine.Object
    {
        [SerializeField] protected TOwner Owner;
        
        protected IState<TOwner> _currentState;
        protected IState<TOwner> _previousState;

        public StateMachine() {
            StateMachineRunner.Register(this);
        }

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

        public void EarlyUpdate()
        {
            _currentState?.OnEarlyUpdate(Owner);
        }

        public void Update()
        {
            _currentState?.OnUpdate(Owner);
        }

        public void LateUpdate()
        {
            _currentState?.OnLateUpdate(Owner);
        }

        public void FixedUpdate()
        {
            _currentState?.OnFixedUpdate(Owner);
        }
    }

    public interface IStateMachine
    {
        void EarlyUpdate();
        void Update();
        void LateUpdate();
        void FixedUpdate();
    }
}
