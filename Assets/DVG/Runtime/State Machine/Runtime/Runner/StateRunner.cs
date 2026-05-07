using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace DVG.StateMachine
{
    internal interface IStateRunner
    {
        public void Run();
        public bool Register<TOwner>(State<TOwner> state) where TOwner : MonoBehaviour;
        public void Unregister<TOwner>(State<TOwner> state) where TOwner : MonoBehaviour;
    }
    
    internal abstract class StateRunner<TUpdate> : IStateRunner where TUpdate : class, IStateStatus
    {
        private const int _initActiveSize = 128;
        
        protected TUpdate[] _states = new TUpdate[_initActiveSize];
        protected int _tailIndex;

        public void Run()
        {
            Span<TUpdate> stateSpan = _states.AsSpan(0, _tailIndex);
            for (int i = 0; i < stateSpan.Length; ++i)
            {
                var state = stateSpan[i];
                TickState(state);
                if (state.IsFinished)
                {
                    stateSpan[i] = null;
                }
            }
        }
        
        protected abstract void TickState(TUpdate state);

        public bool Register<TOwner>(State<TOwner> state) where TOwner : MonoBehaviour
        {
            if (state is not TUpdate u) return false;
            
            AddState(u);
            return true;
        }

        public void Unregister<TOwner>(State<TOwner> state) where TOwner : MonoBehaviour
        {
            state.
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void AddState(TUpdate state)
        {
            if (_states.Length == _tailIndex)
            {
                Array.Resize(ref _states, _tailIndex * 2);
            }
            _states[_tailIndex++] = state;
        }
    }
    
    internal sealed class StateRunnerEarlyUpdate : StateRunner<IEarlyUpdate>
    {
        protected override void TickState(IEarlyUpdate state)
        {
            throw new NotImplementedException();
        }
    }
    
    internal sealed class StateRunnerFixedUpdate : StateRunner<IFixedUpdate>
    {
        protected override void TickState(IFixedUpdate state)
        {
            throw new NotImplementedException();
        }
    }
    
    internal sealed class StateRunnerUpdate : StateRunner<IUpdate>
    {
        protected override void TickState(IUpdate state)
        {
            throw new NotImplementedException();
        }
    }
    
    internal sealed class StateRunnerPreLateUpdate : StateRunner<ILateUpdate>
    {
        protected override void TickState(ILateUpdate state)
        {
            throw new NotImplementedException();
        }
    }
    
    internal sealed class StateRunnerPostLateUpdate : StateRunner<IPostLateUpdate>
    {
        protected override void TickState(IPostLateUpdate state)
        {
            throw new NotImplementedException();
        }
    }
}