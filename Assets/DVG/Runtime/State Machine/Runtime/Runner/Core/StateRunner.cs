using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DVG.StateMachine.Editor;
using UnityEngine;

namespace DVG.StateMachine
{
    internal interface IStateRunner
    {
        public void Run(float deltaTime);
        public bool Register<TOwner>(State<TOwner> state) where TOwner : MonoBehaviour;
        // public void Unregister<TOwner>(State<TOwner> state) where TOwner : MonoBehaviour;
    }
    
    internal abstract class StateRunner<TUpdate> : IStateRunner where TUpdate : IState
    {
        private const int _initActiveSize = 128;
        private const int _initPendingAddSize = 64;
        
        protected TUpdate[] _states = new TUpdate[_initActiveSize];
        protected int _tailIndex;
        protected LiteStack<TUpdate> _pendingAddStack = new(_initPendingAddSize);

        public void Run(float deltaTime)
        {
            int j = _tailIndex - 1;
            
            Span<TUpdate> stateSpan = _states.AsSpan();
            for (int i = 0; i < stateSpan.Length; ++i)
            {
                TUpdate state = stateSpan[i];
                if(state != null)
                {
                    try
                    {
                        TickState(state, deltaTime);
                        if (state.IsFinished) stateSpan[i] = default;
                        else continue;
                    }
                    catch (Exception e)
                    {
                        stateSpan[i] = default;
                        Debug.LogException(e);
                    }
                }

                while (i < j)
                {
                    TUpdate fromTail = stateSpan[j];
                    if (fromTail != null)
                    {
                        try
                        {
                            TickState(fromTail, deltaTime);
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
                break;

                NEXT_LOOP:
                continue;
            }

            while (_pendingAddStack.Count > 0)
            {
                if (_states.Length == _tailIndex)
                {
                    Array.Resize(ref _states, _tailIndex * 2);
                }
                _states[_tailIndex++] = _pendingAddStack.Pop();
            }
        }

        protected abstract void TickState(TUpdate state, float deltaTime);

        public bool Register<TOwner>(State<TOwner> state) where TOwner : MonoBehaviour
        {
            if (state is not TUpdate u) return false;
            
            _pendingAddStack.Push(u);
            return true;
        }
    }
}