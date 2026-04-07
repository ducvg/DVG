using System.Collections.Generic;

namespace DVG.StateMachine
{
    public static class StateRunner
    {
        private static readonly HashSet<IStateMachine> _stateMachinesToAdd = new(64);
        private static readonly HashSet<IStateMachine> _stateMachinesToRemove = new(64);

        private static readonly HashSet<IStateMachine> _activeStateMachines = new(256);

        public static void EarlyUpdate() 
        {
            ApplyPendingChanges();
            foreach(var stateMachine in _activeStateMachines) stateMachine.EarlyUpdate();
        }

        public static void Update() 
        {
            foreach(var stateMachine in _activeStateMachines) stateMachine.Update();
        }

        public static void LateUpdate() 
        {
            foreach(var stateMachine in _activeStateMachines) stateMachine.LateUpdate();
        }

        public static void FixedUpdate() 
        {
            foreach(var stateMachine in _activeStateMachines) stateMachine.FixedUpdate();
        }

        public static void Register<TStateMachine>(TStateMachine stateMachine)
            where TStateMachine : IStateMachine
        {
            _stateMachinesToRemove.Remove(stateMachine);
            _stateMachinesToAdd.Add(stateMachine);
        }

        public static void Deregister<TStateMachine>(TStateMachine stateMachine)
            where TStateMachine : IStateMachine
        {
            _stateMachinesToAdd.Remove(stateMachine);
            _stateMachinesToRemove.Add(stateMachine);
        }

        private static void ApplyPendingChanges()
        {
            foreach(var stateMachine in _stateMachinesToAdd) _activeStateMachines.Add(stateMachine);
            _stateMachinesToAdd.Clear();

            foreach(var stateMachine in _stateMachinesToRemove) _activeStateMachines.Remove(stateMachine);
            _stateMachinesToRemove.Clear();
        }
    }
}