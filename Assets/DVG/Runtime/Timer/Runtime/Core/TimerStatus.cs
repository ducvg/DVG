namespace DVG.Timers
{
	internal enum TimerStatus : byte
	{
		Created, //wait for run()
		Preserved,

		ProgressTick,

		Running,
		Paused,
		Stopped,
		Finished,

		NewLoop,
		
		Disposed
	}
}