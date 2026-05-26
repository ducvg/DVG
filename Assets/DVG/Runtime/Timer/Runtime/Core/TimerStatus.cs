namespace DVG.Timers
{
	internal enum TimerStatus : byte
	{
		Created,

		Running,
		Paused,
		Completed,

		NewLoop,
		
		Preserved,
		Disposed
	}
}