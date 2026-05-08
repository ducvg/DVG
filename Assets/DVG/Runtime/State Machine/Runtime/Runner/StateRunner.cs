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
    
    internal abstract class StateRunner<TUpdate> : IStateRunner where TUpdate : IStateStatus
    {
        private const int _initActiveSize = 128;
        
        protected TUpdate[] _states = new TUpdate[_initActiveSize];
        protected int _tailIndex;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddState(TUpdate state)
        {
            if (_states.Length == _tailIndex)
            {
                Array.Resize(ref _states, _tailIndex * 2);
            }
            _states[_tailIndex++] = state;
        }

        public void Run()
        {
            Span<TUpdate> stateSpan = _states.AsSpan();
            for (int i = 0; i < stateSpan.Length; ++i)
            {
                var state = stateSpan[i];
                if(state != null)
                {
                    try
                    {
                        TickState(state);
                        if (state.IsFinished) _states[i] = default;
                        else continue;
                    }
                    catch (Exception e)
                    {
                        _states[i] = default;
                        Debug.LogException(e);
                    }
                }

                int j = _tailIndex - 1;
                while (i < j)
                {
                    var fromTail = stateSpan[j];
                    if (fromTail != null)
                    {
                        try
                        {
                            TickState(fromTail);
                            if (fromTail.IsFinished)
                            {
                                stateSpan[j] = default;
                                j--;
                                continue; // next j
                            }
                            else
                            {
                                // swap
                                stateSpan[i] = fromTail;
                                stateSpan[j] = default;
                                j--;
                                goto NEXT_LOOP; // next i
                            }
                        }
                        catch (Exception ex)
                        {
                            stateSpan[j] = default;
                            j--;
                            Debug.LogException(ex);
                            continue; // next j
                        }
                    }
                    else
                    {
                        j--;
                    }
                }

                _tailIndex = i; // loop end

                NEXT_LOOP:
                continue;
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected abstract void TickState(TUpdate state);

        public bool Register<TOwner>(State<TOwner> state) where TOwner : MonoBehaviour
        {
            if (state is not TUpdate u) return false;
            
            AddState(u);
            return true;
        }

        public void Unregister<TOwner>(State<TOwner> state) where TOwner : MonoBehaviour
        {
            state.IsFinished = true;
        }

    }
    
    internal sealed class StateRunnerEarlyUpdate : StateRunner<IEarlyUpdate>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void TickState(IEarlyUpdate state)
        {
            state.EarlyUpdate();
        }
    }
    
    internal sealed class StateRunnerFixedUpdate : StateRunner<IFixedUpdate>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void TickState(IFixedUpdate state)
        {
            state.FixedUpdate();
        }
    }
    
    internal sealed class StateRunnerUpdate : StateRunner<IUpdate>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void TickState(IUpdate state)
        {
            state.Update();
        }
    }
    
    internal sealed class StateRunnerPreLateUpdate : StateRunner<ILateUpdate>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void TickState(ILateUpdate state)
        {
            state.LateUpdate();
        }
    }
    
    internal sealed class StateRunnerPostLateUpdate : StateRunner<IPostLateUpdate>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void TickState(IPostLateUpdate state)
        {
            state.PostLateUpdate();
        }
    }
}