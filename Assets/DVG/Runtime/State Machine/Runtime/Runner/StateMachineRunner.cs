using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DVG.StateMachine
{
    public static class StateMachineRunner
    {
        private static readonly HashSet<IStateMachine> _stateMachinesToAdd = new(64);
        private static readonly HashSet<IStateMachine> _stateMachinesToRemove = new(64);

        private static readonly HashSet<IStateMachine> _activeStateMachines = new(256);

        public static void EarlyUpdate() 
        {
            foreach(var stateMachine in _stateMachinesToAdd) _activeStateMachines.Add(stateMachine);
            _stateMachinesToAdd.Clear();

            foreach(var stateMachine in _stateMachinesToRemove) _activeStateMachines.Remove(stateMachine);
            _stateMachinesToRemove.Clear();

            foreach(var stateMachine in _activeStateMachines) stateMachine.Update();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Register<TStateMachine>(TStateMachine stateMachine) where TStateMachine : IStateMachine
            => _stateMachinesToAdd.Add(stateMachine);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Deregister<TStateMachine>(TStateMachine stateMachine) where TStateMachine : IStateMachine
            => _stateMachinesToRemove.Add(stateMachine);
    }
}