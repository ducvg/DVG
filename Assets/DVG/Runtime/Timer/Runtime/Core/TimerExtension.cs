using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace DVG.Timers
{
	//gay shit
	public static class TimerExtension
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Timer BindTo(this Timer timer, MonoBehaviour owner)
		{
			ref var managedData = ref timer.DataStorage.GetManagedDataRef(timer);
			managedData.lifeLinkedCancellationToken = owner.destroyCancellationToken;
			return timer;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Timer OnStart(this Timer timer, Action onStartAction)
		{
			ref var managedData = ref timer.DataStorage.GetManagedDataRef(timer);
			managedData.StartActionContext = null;
			managedData.OnStartAction = onStartAction;
			return timer;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Timer OnStart<T>(this Timer timer, 
			T context, Action<T> onStartAction)
		{
			ref var managedData = ref timer.DataStorage.GetManagedDataRef(timer);
			managedData.StartActionContext = context;
			managedData.OnStartAction = onStartAction;
			return timer;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Timer OnTick(this Timer timer, Action<float> onTickAction)
		{
			ref var managedData = ref timer.DataStorage.GetManagedDataRef(timer);
			managedData.TickActionContext = null;
			managedData.OnTickAction = onTickAction;
			return timer;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Timer OnTick<T>(this Timer timer, 
			T context, Action<T, float> onTickAction)
		{
			ref var managedData = ref timer.DataStorage.GetManagedDataRef(timer);
			managedData.TickActionContext = context;
			managedData.OnTickAction = onTickAction;
			return timer;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Timer OnLoopComplete(this Timer timer, 
			Action<int, float> onLoopCompleteAction)
		{
			ref var managedData = ref timer.DataStorage.GetManagedDataRef(timer);
			managedData.LoopCompleteActionContext = null;
			managedData.OnLoopCompleteAction = onLoopCompleteAction;
			return timer;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Timer OnLoopComplete<T>(this Timer timer, 
			T context, Action<T, int, float> onLoopCompleteAction)
		{
			ref var managedData = ref timer.DataStorage.GetManagedDataRef(timer);
			managedData.LoopCompleteActionContext = context;
			managedData.OnLoopCompleteAction = onLoopCompleteAction;
			return timer;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Timer OnPause(this Timer timer, Action<float> onPauseAction)
		{
			ref var managedData = ref timer.DataStorage.GetManagedDataRef(timer);
			managedData.PauseActionContext = null;
			managedData.OnPauseAction = onPauseAction;
			return timer;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Timer OnPause<T>(this Timer timer, 
			T context, Action<T, float> onPauseAction)
		{
			ref var managedData = ref timer.DataStorage.GetManagedDataRef(timer);
			managedData.PauseActionContext = context;
			managedData.OnPauseAction = onPauseAction;
			return timer;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Timer OnContinue(this Timer timer, Action<float> onContinueAction)
		{
			ref var managedData = ref timer.DataStorage.GetManagedDataRef(timer);
			managedData.ContinueActionContext = null;
			managedData.OnContinueAction = onContinueAction;
			return timer;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Timer OnContinue<T>(this Timer timer, 
			T context, Action<T, float> onContinueAction)
		{
			ref var managedData = ref timer.DataStorage.GetManagedDataRef(timer);
			managedData.ContinueActionContext = context;
			managedData.OnContinueAction = onContinueAction;
			return timer;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Timer OnStop(this Timer timer, Action onStopAction)
		{
			ref var managedData = ref timer.DataStorage.GetManagedDataRef(timer);
			managedData.StopActionContext = null;
			managedData.OnStopAction = onStopAction;
			return timer;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Timer OnStop<T>(this Timer timer, 
			T context, Action<T> onStopAction)
		{
			ref var managedData = ref timer.DataStorage.GetManagedDataRef(timer);
			managedData.StopActionContext = context;
			managedData.OnStopAction = onStopAction;
			return timer;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Timer OnComplete(this Timer timer, Action onCompleteAction)
		{
			ref var managedData = ref timer.DataStorage.GetManagedDataRef(timer);
			managedData.CompleteActionContext = null;
			managedData.OnCompleteAction = onCompleteAction;
			return timer;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Timer OnComplete<T>(this Timer timer, 
			T context, Action<T> onCompleteAction)
		{
			ref var managedData = ref timer.DataStorage.GetManagedDataRef(timer);
			managedData.CompleteActionContext = context;
			managedData.OnCompleteAction = onCompleteAction;
			return timer;
		}
	}
}