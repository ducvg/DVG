using System.Runtime.CompilerServices;

namespace DVG.StateMachine
{
    internal sealed class StateRunnerEarlyUpdate : StateRunner<IEarlyUpdate>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void TickState(IEarlyUpdate state, float deltaTime)
        {
            state.EarlyUpdate(deltaTime);
        }
    }
}