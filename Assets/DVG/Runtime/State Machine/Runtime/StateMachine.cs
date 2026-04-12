using System.Threading;
using Codice.Client.BaseCommands;
using UnityEngine;

namespace DVG.StateMachine
{
    //TOwner is the MonoBehaviour that owns the state machine
    public abstract class StateMachine<TOwner> where TOwner : MonoBehaviour
    {
        protected TOwner _owner;

        protected IState<TOwner> _previousState, _currentState;

        private CancellationTokenRegistration _destroyctRegistration;

        public void SetOwner(TOwner owner)
        {
            if(_owner == owner) return;
            if(_owner != null) _destroyctRegistration.Dispose();
            
            _owner = owner;
            _destroyctRegistration = _owner.destroyCancellationToken.Register(OnDestroy);
        }

        public void ChangeState<TState>(TState newState) where TState : IState<TOwner>
        {
            StateRunner.Deregister(_currentState);
            _currentState?.OnExit(_owner);
            
            _previousState = _currentState;
            _currentState = newState;
            
            _currentState?.OnEnter(_owner);
            StateRunner.Register(_currentState);

            #if UNITY_EDITOR
            AddToHistory(_currentState);
            #endif
        }

        public void ClearState()
        {
            StateRunner.Deregister(_currentState);
            _currentState?.OnExit(_owner);
            
            _previousState = _currentState = null;
        }

        private void OnDestroy()
        {
            if(_currentState != null) StateRunner.Deregister(_currentState);
        }

#region Debug
        #if UNITY_EDITOR
        [SerializeField] private string[] _statesHistory = new string[_historySize];
        private int _historyIndex = 0;
        private const int _historySize = 16;

        private void AddToHistory(IState<TOwner> state)
        {
            _statesHistory[_historyIndex] = state.GetType().Name;
            _historyIndex = (_historyIndex + 1) % _historySize;
        }

        #endif
#endregion
    }
}
