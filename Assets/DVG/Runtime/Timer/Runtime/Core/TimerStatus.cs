namespace DVG.Timers
{
	internal enum TimerStatus : byte
	{
		Created, //wait for run()
		Preserved,

		AccumulateTick,

		Running,
		Paused,
		Stopped,
		Finished,

		NewLoop,
		
		Disposed
	}
}