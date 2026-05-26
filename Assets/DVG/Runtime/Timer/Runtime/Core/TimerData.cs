using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.Mathematics;

namespace DVG.Timers
{
	internal struct TimerData
    {
        public TimerStatus Status;
        public TimerTiming Timing;
		
		public bool IsPreserved;

        public int Loops;
		public int CompletedLoops => Duration > 0f ? (int)(ElapsedTime / Duration + math.EPSILON) : 0;

        public float Duration;
        public float ElapsedTime;
		public float LoopElapsedTime => Status == TimerStatus.Completed ? Duration : ElapsedTime % Duration;
        public float TickRateSeconds;
		public float TickProgress;
    }

	internal struct TimerManagedData
    {
		internal CancellationToken bindOwnerCancellationToken;

        internal object StartActionContext, TickActionContext, CompleteActionContext;
        internal object OnStartAction, OnTickAction, OnCompleteAction;
		internal object PauseActionContext, ContinueActionContext;
		internal object OnPauseAction, OnContinueAction;
		internal object LoopCompleteActionContext, OnLoopCompleteAction;
		internal object DisposeActionContext, OnDisposeAction;

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
		internal void OnPause(float totalElapsedTime)
		{
			if(OnPauseAction == null) return;

			if(PauseActionContext != null)
			{
				Unsafe.As<Action<object, float>>(OnPauseAction).Invoke(PauseActionContext, totalElapsedTime);
			} else
			{
				Unsafe.As<Action<float>>(OnPauseAction).Invoke(totalElapsedTime);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void OnContinue(float totalElapsedTime)
		{
			if(OnContinueAction == null) return;

			if(ContinueActionContext != null)
			{
				Unsafe.As<Action<object, float>>(OnContinueAction).Invoke(ContinueActionContext, totalElapsedTime);
			} else
			{
				Unsafe.As<Action<float>>(OnContinueAction).Invoke(totalElapsedTime);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void OnDisposed()
		{
			if(OnDisposeAction == null) return;

			if(DisposeActionContext != null)
			{
				Unsafe.As<Action<object>>(OnDisposeAction).Invoke(DisposeActionContext);
			} else
			{
				Unsafe.As<Action>(OnDisposeAction).Invoke();
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