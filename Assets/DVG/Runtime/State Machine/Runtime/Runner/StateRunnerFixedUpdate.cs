using System.Runtime.CompilerServices;

namespace DVG.StateMachine
{
    internal sealed class StateRunnerFixedUpdate : StateRunner<IFixedUpdate>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void TickState(IFixedUpdate state, float deltaTime)
        {
            state.FixedUpdate(deltaTime);
        }
    }
}