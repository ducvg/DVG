using System.Runtime.CompilerServices;

namespace DVG.StateMachine
{
    internal sealed class StateRunnerPreLateUpdate : StateRunner<ILateUpdate>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void TickState(ILateUpdate state, float deltaTime)
        {
            state.LateUpdate(deltaTime);
        }
    }
}