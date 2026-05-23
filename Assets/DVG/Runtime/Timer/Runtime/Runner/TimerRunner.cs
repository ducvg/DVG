using System.Runtime.CompilerServices;
using Unity.Burst.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;

namespace DVG.Timers
{
    public sealed class TimerRunner
    {
		internal readonly TimerDataStorage DataStorage;

		internal TimerRunner(TimerDataStorage dataStorage)
		{
			DataStorage = dataStorage;
		}

        internal unsafe void Update(float scaledDeltaTime, float unscaledDeltaTime)
        {
			var count = DataStorage.Count;
			using NativeList<int> stopTimerIndexs = new NativeList<int>(count, Allocator.TempJob);
			fixed (TimerData* dataPtr = DataStorage.GetDataSpan())
			{
				new UpdateTimerJob
				{
					ScaledDeltaTime = scaledDeltaTime,
					UnscaledDeltaTime = unscaledDeltaTime,
					RemoveTimerIndexs = stopTimerIndexs.AsParallelWriter(),
					DataArrayPtr = dataPtr,
				}
				.Schedule(count, TimerSettings.JobBatchCount).Complete();

				var managedDataSpan = DataStorage.GetManagedDataSpan();
                for (int i = 0; i < count; ++i)
				{
					TimerData* timerDataPtr = dataPtr + i;
					var timerManagedData = managedDataSpan[i];

					if(timerManagedData.lifeLinkedCancellationToken.IsCancellationRequested)
					{
						timerDataPtr->Status = TimerStatus.Stopped;
						timerManagedData.OnStop();
						stopTimerIndexs.AddNoResize(i);
						continue;
					}

					switch (timerDataPtr->Status)
					{
						case TimerStatus.Running:
							timerManagedData.OnTick(timerDataPtr->ElapsedTime);
							break;
						case TimerStatus.Finished:
							if(timerDataPtr->IsPreserved) timerDataPtr->Status = TimerStatus.Preserved;
							timerManagedData.OnComplete();
							break;
						case TimerStatus.Stopped:
							if(timerDataPtr->IsPreserved) timerDataPtr->Status = TimerStatus.Preserved;
							timerManagedData.OnStop();
							break;
						case TimerStatus.NewLoop:
							timerManagedData.OnLoopComplete(timerDataPtr->CompletedLoops, timerDataPtr->ElapsedTime, timerDataPtr->CycleElapsedTime);
							timerDataPtr->Status = TimerStatus.Running;
							break;
					}
				}
			}
			
			DataStorage.RemoveAll(stopTimerIndexs);
        }

        private unsafe struct UpdateTimerJob : IJobParallelFor
        {
			[ReadOnly] public float ScaledDeltaTime, UnscaledDeltaTime;
			[WriteOnly] public NativeList<int>.ParallelWriter RemoveTimerIndexs;
			[NativeDisableUnsafePtrRestriction] public TimerData* DataArrayPtr;

            public void Execute([AssumeRange(0, int.MaxValue)] int dataIndex)
            {
				var timerDataPtr = DataArrayPtr + dataIndex;
				if(Hint.Unlikely(timerDataPtr->Status == TimerStatus.Disposed))
				{
					RemoveTimerIndexs.AddNoResize(dataIndex);
					return;
				}
				if(timerDataPtr->Status is TimerStatus.Paused or TimerStatus.Created or TimerStatus.Preserved) return;
				if(Hint.Unlikely(timerDataPtr->Status == TimerStatus.Stopped))
				{
					StopTimer(timerDataPtr, dataIndex);
					return;
				}

				float deltaTime = timerDataPtr->Timing == TimerTiming.ScaleTime ? ScaledDeltaTime : UnscaledDeltaTime;
				float timerDuration = timerDataPtr->Duration;
				float tickRateSeconds = timerDataPtr->TickRateSeconds;
				float newElapsedTime = timerDataPtr->ElapsedTime + deltaTime;

				if(tickRateSeconds > 0)
				{
					float invTickRate = 1f / tickRateSeconds;
					float prevElapsed = timerDataPtr->ElapsedTime;

					uint prevTickCount = (uint)(prevElapsed * invTickRate);
					uint newTickCount = (uint)(newElapsedTime * invTickRate);

					if(Hint.Likely(newTickCount == prevTickCount))
					{
						timerDataPtr->Status = TimerStatus.AccumulateTick;
						return;
					}
					else
					{
						timerDataPtr->Status = TimerStatus.Running;
					}
				}

				timerDataPtr->ElapsedTime = newElapsedTime;
				timerDataPtr->CycleElapsedTime += deltaTime;
				            
				float loopCount = timerDataPtr->Loops;
				if(Hint.Unlikely(timerDataPtr->CycleElapsedTime >= timerDuration))
				{
					var completedLoops = timerDataPtr->CompletedLoops + 1;
					if(loopCount <= 0 || Hint.Unlikely(completedLoops > loopCount))
					{
						if(newElapsedTime >= timerDuration) timerDataPtr->ElapsedTime = timerDuration;
						StopTimer(timerDataPtr, dataIndex);
						return;
					}

					timerDataPtr->CompletedLoops++;
					timerDataPtr->CycleElapsedTime = 0;
					timerDataPtr->Status = TimerStatus.NewLoop;
				}
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private void StopTimer(TimerData* timerDataPtr, int dataIndex)
			{
				timerDataPtr->Status = TimerStatus.Stopped;
				if(timerDataPtr->IsPreserved) return;
				RemoveTimerIndexs.AddNoResize(dataIndex);
			}
        }

		internal void Dispose()
		{
			DataStorage.Clear();
		}
    }
}