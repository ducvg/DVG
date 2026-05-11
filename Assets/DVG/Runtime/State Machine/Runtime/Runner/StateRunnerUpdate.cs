using System.Runtime.CompilerServices;

namespace DVG.StateMachine
{
    internal sealed class StateRunnerUpdate : StateRunner<IUpdate>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void TickState(IUpdate state, float deltaTime)
        {
            state.Update(deltaTime);
        }
    }
}