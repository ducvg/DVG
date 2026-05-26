using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace DVG.Timers
{
    public readonly struct Timer
    {
		public float Duration
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => DataStorage.GetDataRef(this).Duration;
		}
		public float ElapsedTime
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => DataStorage.GetDataRef(this).ElapsedTime;
		}
		public int Loops
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => DataStorage.GetDataRef(this).Loops;
		}
		public int CompletedLoops
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => DataStorage.GetDataRef(this).CompletedLoops;
		}
		public float CycleElapsedTime
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => DataStorage.GetDataRef(this).LoopElapsedTime;
		}

		public bool IsRunning
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				TimerData data = DataStorage.GetDataRef(this);
				return data.Status == TimerStatus.Running || data.Status == TimerStatus.NewLoop;
			}
		}

		public bool IsComplete
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				TimerData data = DataStorage.GetDataRef(this);
				return data.ElapsedTime >= data.Duration * (data.Loops + 1);
			}
		}

		public bool IsPaused
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				TimerData data = DataStorage.GetDataRef(this);
				return data.Status == TimerStatus.Paused;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Run()
		{
			ref TimerData data = ref DataStorage.GetDataRef(this);
			ref TimerManagedData managedData = ref DataStorage.GetManagedDataRef(this);

			switch(data.Status)
			{
				case TimerStatus.Created:
					data.Status = TimerStatus.Running;
					managedData.OnStart();
					return;
				case TimerStatus.Paused:
					data.Status = TimerStatus.Running;
					managedData.OnContinue(data.ElapsedTime);
					return;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Pause()
		{
			ref TimerData data = ref DataStorage.GetDataRef(this);
			ref TimerManagedData managedData = ref DataStorage.GetManagedDataRef(this);

			if(data.Status is TimerStatus.Running or TimerStatus.Created or TimerStatus.NewLoop)
			{
				data.Status = TimerStatus.Paused;
				managedData.OnPause(data.ElapsedTime);
			}
		}

		public void Complete()
		{
			ref TimerData data = ref DataStorage.GetDataRef(this);
			ref TimerManagedData managedData = ref DataStorage.GetManagedDataRef(this);

			if(data.Status is TimerStatus.Created or TimerStatus.Running or TimerStatus.Paused or TimerStatus.NewLoop)
			{
				data.ElapsedTime = data.Duration * (data.Loops + 1);
				data.TickProgress = data.TickRateSeconds;
				managedData.OnComplete();

				data.Status = data.IsPreserved ? TimerStatus.Preserved : TimerStatus.Completed;
			}
		}

		public void Reset()
		{
			ref TimerData data = ref DataStorage.GetDataRef(this);
			data.Status = TimerStatus.Created;
			data.ElapsedTime = 0;
		}

		public void Reset(float newDuration)
		{
			ref TimerData data = ref DataStorage.GetDataRef(this);
			data.Status = TimerStatus.Created;
			data.Duration = newDuration;
			data.ElapsedTime = 0;
		}

		public void ResetTime()
		{
			ref TimerData data = ref DataStorage.GetDataRef(this);
			data.ElapsedTime = 0;
		}

		public void ResetTime(float newDuration)
		{
			ref TimerData data = ref DataStorage.GetDataRef(this);
			data.Duration = newDuration;
			data.ElapsedTime = 0;
		}

		public void Dispose()
		{
			ref TimerData data = ref DataStorage.GetDataRef(this);
			data.Status = TimerStatus.Disposed;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Timer Create(
			float duration, float tickRateSeconds = -1f, 
			int loops = 0, bool preserve = false,
			TimerTiming timing = TimerTiming.ScaleTime,
			TimerRunner updater = null
		)
		{
			updater ??= TimerUpdater.UpdateRunner;
			TimerData data = new TimerData()
			{
				Status = TimerStatus.Created,
				PrevStatus = TimerStatus.Created,
				Duration = duration,
				TickRateSeconds = tickRateSeconds,
				Timing = timing,
				Loops = loops,
				IsPreserved = preserve,
				ElapsedTime = 0,
			};
			
			TimerManagedData managedData = new TimerManagedData();

			return updater.DataStorage.Create(ref data, ref managedData);
		}

        internal readonly int Version;
        internal readonly int DataSparseIndex;
        internal readonly TimerDataStorage DataStorage;

		internal Timer(int version, int dataSparseIndex, TimerDataStorage dataStorage)
		{
			Version = version;
			DataSparseIndex = dataSparseIndex;
			DataStorage = dataStorage;
		}
	}
}