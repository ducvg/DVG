namespace DVG.Timers
{
	internal enum TimerStatus : byte
	{
		Created, //wait for run()
		Preserved,

		Running,
		Paused,
		Completed,

		NewLoop,
		
		Disposed
	}
}