using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace DVG.StateMachine
{
    internal sealed class StateRunnerPostLateUpdate : StateRunner<IPostLateUpdate>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void TickState(IPostLateUpdate state, float deltaTime)
        {
            state.PostLateUpdate(deltaTime);
        }
    }
}