namespace DVG.Timers
{
    public static class TimerUpdater
    {
		public static TimerRunner EarlyUpdate;
		public static TimerRunner FixedUpdate;
		public static TimerRunner UpdateRunner;
		public static TimerRunner PreLateUpdate;
		public static TimerRunner PostLateUpdate;

		public static TimerRunner LateUpdate => PreLateUpdate;
    }
}