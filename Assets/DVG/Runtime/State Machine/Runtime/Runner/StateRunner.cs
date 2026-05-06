using System;

namespace DVG.StateMachine
{
    internal interface IStateRunner
    {
        public void Run();
    }
    
    internal abstract class StateRunner<TUpdate> : IStateRunner
    {
        private const int _initActiveSize = 128;
        
        public readonly UpdateType StateRunnerUpdateType;
        protected readonly TUpdate[] _states;
        protected int _tailIndex;

        protected bool _isRunning;
        
        public StateRunner(UpdateType updateType)
        {
            StateRunnerUpdateType  = updateType;
            _states = new TUpdate[_initActiveSize];
            _tailIndex = 0;
            _isRunning = false;
        }
        
        public abstract void Run();
        public abstract void Register<TState>(TState state);
        public abstract void Unregister<TState>(TState state);
    }
    
    internal sealed class StateRunnerEarlyUpdate : StateRunner<IEarlyUpdate>
    {
        public StateRunnerEarlyUpdate() : base(UpdateType.EarlyUpdate){}

        public override void Run()
        {
            _isRunning = true;
            Span<IEarlyUpdate> stateSpan = _states.AsSpan(0, _tailIndex);
            for (int i = 0; i < stateSpan.Length; ++i)
            {
                stateSpan[i].EarlyUpdate();
            }
            _isRunning = false;
        }

        public override void Register<TState>(TState state)
        {
            if (state is not IEarlyUpdate) return;
        }

        public override void Unregister<TState>(TState state)
        {
            throw new NotImplementedException();
        }
    }
    
    internal sealed class StateRunnerFixedUpdate : StateRunner<IFixedUpdate>
    {
        public StateRunnerFixedUpdate() : base(UpdateType.FixedUpdate){}
        
        public override void Run()
        {
            _isRunning = true;
            Span<IFixedUpdate> stateSpan = _states.AsSpan(0, _tailIndex);
            for (int i = 0; i < stateSpan.Length; ++i)
            {
                stateSpan[i].FixedUpdate();
            }
            _isRunning = false;
        }
    }
    
    internal sealed class StateRunnerUpdate : StateRunner<IUpdate>
    {
        public StateRunnerUpdate() : base(UpdateType.Update){}
        
        public override void Run()
        {
            _isRunning = true;
            Span<IUpdate> stateSpan = _states.AsSpan(0, _tailIndex);
            for (int i = 0; i < stateSpan.Length; ++i)
            {
                stateSpan[i].Update();
            }
            _isRunning = false;
        }
    }
    
    internal sealed class StateRunnerPreLateUpdate : StateRunner<ILateUpdate>
    {
        public StateRunnerPreLateUpdate() : base(UpdateType.LateUpdate){}
        
        public override void Run()
        {
            _isRunning = true;
            Span<ILateUpdate> stateSpan = _states.AsSpan(0, _tailIndex);
            for (int i = 0; i < stateSpan.Length; ++i)
            {
                stateSpan[i].LateUpdate();
            }
            _isRunning = false;
        }
    }
    
    internal sealed class StateRunnerPostLateUpdate : StateRunner<IPostLateUpdate>
    {
        public StateRunnerPostLateUpdate() : base(UpdateType.PostLateUpdate){}
        
        public override void Run()
        {
            _isRunning = true;
            Span<IPostLateUpdate> stateSpan = _states.AsSpan(0, _tailIndex);
            for (int i = 0; i < stateSpan.Length; ++i)
            {
                stateSpan[i].PostLateUpdate();
            }
            _isRunning = false;
        }
    }
}