using System.Collections.Generic;

namespace DVG.StateMachine
{
    public static class StateRunner
    {
        private const int _initModifySize = 64;
        private static readonly HashSet<object> _addStates = new(_initModifySize);
        private static readonly HashSet<object> _removeStates = new(_initModifySize);

        private const int _initActiveSize = 256;
        private static readonly HashSet<IEarlyUpdate> _earlyUpdateStates = new(_initActiveSize);
        private static readonly HashSet<IUpdate> _updateStates = new(_initActiveSize);
        private static readonly HashSet<ILateUpdate> _lateUpdateStates = new(_initActiveSize);
        private static readonly HashSet<IFixedUpdate> _fixedUpdateStates = new(_initActiveSize);

        public static void EarlyUpdate()
        {
            HandleRegister();
            HandleDeregister();
            foreach(var stateMachine in _earlyUpdateStates) stateMachine.EarlyUpdate();
        }

        public static void Update() 
        {
            foreach(var stateMachine in _updateStates) stateMachine.Update();
        }

        public static void PreLateUpdate() 
        {
            foreach(var stateMachine in _lateUpdateStates) stateMachine.LateUpdate();
        }

        public static void FixedUpdate() 
        {
            foreach(var stateMachine in _fixedUpdateStates) stateMachine.FixedUpdate();
        }

        public static void Register(object state)
        {
            _removeStates.Remove(state);
            _addStates.Add(state);
        }

        public static void Deregister(object state)
        {
            _addStates.Remove(state);
            _removeStates.Add(state);
        }

        //500 cigarettes
        private static void HandleRegister()
        {
            foreach (object state in _addStates)
            {
                if(state is IEarlyUpdate eu) _earlyUpdateStates.Add(eu);
                if(state is IUpdate u) _updateStates.Add(u);
                if(state is ILateUpdate lu) _lateUpdateStates.Add(lu);
                if(state is IFixedUpdate fu) _fixedUpdateStates.Add(fu);
            }
            _addStates.Clear();
        }

        private static void HandleDeregister()
        {
            foreach (object state in _removeStates)
            {
                if(state is IEarlyUpdate eu) _earlyUpdateStates.Remove(eu);
                if(state is IUpdate u) _updateStates.Remove(u);
                if(state is ILateUpdate lu) _lateUpdateStates.Remove(lu);
                if(state is IFixedUpdate fu) _fixedUpdateStates.Remove(fu);
            }
            _removeStates.Clear();
        }
    }
}