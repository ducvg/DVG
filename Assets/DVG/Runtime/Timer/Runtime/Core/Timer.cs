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
			get => DataStorage.GetDataRef(this).CycleElapsedTime;
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
				return data.Status == TimerStatus.Finished;
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

        internal readonly int Version;
        internal readonly int DataSparseIndex;
        internal readonly TimerDataStorage DataStorage;

		internal Timer(int version, int dataSparseIndex, TimerDataStorage dataStorage)
		{
			Version = version;
			DataSparseIndex = dataSparseIndex;
			DataStorage = dataStorage;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Run()
		{
			ref TimerData data = ref DataStorage.GetDataRef(this);
			ref TimerManagedData managedData = ref DataStorage.GetManagedDataRef(this);

			switch(data.Status)
			{
				case TimerStatus.Paused:
					data.Status = TimerStatus.Running;
					managedData.OnContinue(data.ElapsedTime);
					return;
				case TimerStatus.Stopped:
					data.Status = TimerStatus.Running;
					return;
				case TimerStatus.Created:
					data.Status = TimerStatus.Running;
					managedData.OnStart();
					return;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Pause()
		{
			ref TimerData data = ref DataStorage.GetDataRef(this);
			ref TimerManagedData managedData = ref DataStorage.GetManagedDataRef(this);

			if(data.Status == TimerStatus.Running)
			{
				data.Status = TimerStatus.Paused;
				managedData.OnPause(data.ElapsedTime);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Stop()
		{
			ref TimerData data = ref DataStorage.GetDataRef(this);

			if(data.Status is TimerStatus.Running or TimerStatus.Paused or TimerStatus.NewLoop)
			{
				data.Status = TimerStatus.Stopped;
			}
		}
	
		public void Complete()
		{
			ref TimerData data = ref DataStorage.GetDataRef(this);

			if(data.Status is TimerStatus.Running or TimerStatus.Paused or TimerStatus.NewLoop)
			{
				data.Status = TimerStatus.Finished;
			}
		}

		public void ResetTime()
		{
			ref TimerData data = ref DataStorage.GetDataRef(this);
			data.ElapsedTime = 0;
			data.CycleElapsedTime = 0;
			data.CompletedLoops = 0;
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
				Duration = duration,
				TickRateSeconds = tickRateSeconds,
				Timing = timing,
				Loops = loops,
				IsPreserved = preserve,
				ElapsedTime = 0,
				CycleElapsedTime = 0,
				CompletedLoops = -1
			};
			
			TimerManagedData managedData = new TimerManagedData();

			return updater.DataStorage.Create(ref data, ref managedData);
		}
	}

    internal struct TimerData
    {
        public float Duration;
        public float TickRateSeconds;
        public int Loops;
		public bool IsPreserved;
        public TimerTiming Timing;
		
        public TimerStatus Status;
        public float ElapsedTime;
		public float CycleElapsedTime;
		public int CompletedLoops;
    }

    internal struct TimerManagedData
    {
		internal CancellationToken lifeLinkedCancellationToken;

        internal object StartActionContext, TickActionContext, StopActionContext, CompleteActionContext;
        internal object OnStartAction, OnTickAction, OnStopAction, OnCompleteAction;
		internal object PauseActionContext, ContinueActionContext;
		internal object OnPauseAction, OnContinueAction;
		internal object LoopCompleteActionContext, OnLoopCompleteAction;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void OnStart()
		{
			if(OnStartAction == null) return;

			if(StartActionContext != null)
			{
				Unsafe.As<Action<object>>(OnStartAction).Invoke(StartActionContext);
			} else
			{
				Unsafe.As<Action>(OnStartAction).Invoke();
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void OnTick(float totalElapsedTime)
		{
			if(OnTickAction == null) return;

			if(TickActionContext != null)
			{
				Unsafe.As<Action<object, float>>(OnTickAction).Invoke(TickActionContext, totalElapsedTime);
			} else
			{
				Unsafe.As<Action<float>>(OnTickAction).Invoke(totalElapsedTime);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void OnLoopComplete(int completedLoops, float totalElapsedTime, float cycleElapsedTime)
		{
			if(OnLoopCompleteAction == null) return;

			if(LoopCompleteActionContext != null)
			{
				Unsafe.As<Action<object, int, float, float>>(OnLoopCompleteAction).Invoke(LoopCompleteActionContext, completedLoops, totalElapsedTime, cycleElapsedTime);
			} else
			{
				Unsafe.As<Action<int, float, float>>(OnLoopCompleteAction).Invoke(completedLoops, totalElapsedTime, cycleElapsedTime);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void OnPause(float _totalElapsedTime)
		{
			if(OnPauseAction == null) return;

			if(PauseActionContext != null)
			{
				Unsafe.As<Action<object, float>>(OnPauseAction).Invoke(PauseActionContext, _totalElapsedTime);
			} else
			{
				Unsafe.As<Action<float>>(OnPauseAction).Invoke(_totalElapsedTime);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void OnContinue(float _totalElapsedTime)
		{
			if(OnContinueAction == null) return;

			if(ContinueActionContext != null)
			{
				Unsafe.As<Action<object, float>>(OnContinueAction).Invoke(ContinueActionContext, _totalElapsedTime);
			} else
			{
				Unsafe.As<Action<float>>(OnContinueAction).Invoke(_totalElapsedTime);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void OnStop()
		{
			if(OnStopAction == null) return;

			if(StopActionContext != null)
			{
				Unsafe.As<Action<object>>(OnStopAction).Invoke(StopActionContext);
			} else
			{
				Unsafe.As<Action>(OnStopAction).Invoke();
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void OnComplete()
		{
			if(OnCompleteAction == null) return;

			if(CompleteActionContext != null)
			{
				Unsafe.As<Action<object>>(OnCompleteAction).Invoke(CompleteActionContext);
			} else
			{
				Unsafe.As<Action>(OnCompleteAction).Invoke();
			}
		}
	}
}