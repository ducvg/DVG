namespace DVG.StateMachine
{
    public static class StateManager
    {
        public const int UpdateTypeCount = 5;
        internal static IStateRunner[] Runners = new IStateRunner[UpdateTypeCount];

        public static void Register<TState>(TState state)
        {
            if (state is IEarlyUpdate eu) Runners[(int)UpdateType.EarlyUpdate]. state;
        }
    }
}