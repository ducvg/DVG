using System;

namespace DVG.StateMachine
{
    internal sealed class StateRunnerFixedUpdate : StateRunner<IFixedUpdate>
    {
        public override void Run(float deltaTime, float fixedDeltaTime)
        {
            int j = _tailIndex - 1;
            
            Span<IFixedUpdate> stateSpan = _states.AsSpan();
            for (int i = 0; i < stateSpan.Length; ++i)
            {
                IFixedUpdate state = stateSpan[i];
                if(state != null)
                {
                    try
                    {
                        state.FixedUpdate(fixedDeltaTime);
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
                    IFixedUpdate fromTail = stateSpan[j];
                    if (fromTail != null)
                    {
                        try
                        {
                            fromTail.FixedUpdate(fixedDeltaTime);
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
    }
}