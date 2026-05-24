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
							timerManagedData.OnComplete();
							break;
						case TimerStatus.Stopped:
							timerManagedData.OnStop();
							break;
						case TimerStatus.NewLoop:
							timerManagedData.OnLoopComplete(timerDataPtr->CompletedLoops, timerDataPtr->ElapsedTime, timerDataPtr->LoopElapsedTime);
							timerDataPtr->Status = TimerStatus.Running;
							break;
						case TimerStatus.Preserved:
							switch(timerDataPtr->PrevStatus)
							{
								case TimerStatus.Finished:
									timerManagedData.OnComplete();
									break;
								case TimerStatus.Stopped:
									timerManagedData.OnStop();
									break;
							}
							break;
					}
				}
			}
			
			DataStorage.RemoveAll(stopTimerIndexs);
        }

		internal void Dispose()
		{
			DataStorage.Clear();
		}

        private unsafe struct UpdateTimerJob : IJobParallelFor
        {
			[ReadOnly] public float ScaledDeltaTime, UnscaledDeltaTime;
			[WriteOnly] public NativeList<int>.ParallelWriter RemoveTimerIndexs;
			[NativeDisableUnsafePtrRestriction] public TimerData* DataArrayPtr;

            public void Execute([AssumeRange(0, int.MaxValue)] int dataIndex)
            {
				var timerDataPtr = DataArrayPtr + dataIndex;
				if(timerDataPtr->Status == TimerStatus.Disposed)
				{
					RemoveTimerIndexs.AddNoResize(dataIndex);
					return;
				}
				if(timerDataPtr->Status is TimerStatus.Created or TimerStatus.Paused 
				or TimerStatus.Preserved or TimerStatus.Stopped) return;

				float deltaTime = timerDataPtr->Timing == TimerTiming.ScaleTime ? ScaledDeltaTime : UnscaledDeltaTime;
				float timerDuration = timerDataPtr->Duration;
				float tickRateSeconds = timerDataPtr->TickRateSeconds;
				bool isPreserverd = timerDataPtr->IsPreserved;

				if(Hint.Unlikely(timerDuration <= 0))
				{
					timerDataPtr->ElapsedTime = timerDuration * (timerDataPtr->Loops+1);
					if(isPreserverd)
					{
						timerDataPtr->PrevStatus = TimerStatus.Finished;
						timerDataPtr->Status = TimerStatus.Preserved;
					} 
					else
					{
						timerDataPtr->Status = TimerStatus.Finished;
						RemoveTimerIndexs.AddNoResize(dataIndex);
					}
					return;
				}

				if(tickRateSeconds > math.EPSILON)
				{
					timerDataPtr->TickProgress += deltaTime;

					if(Hint.Likely(timerDataPtr->TickProgress < tickRateSeconds))
					{
						timerDataPtr->Status = TimerStatus.ProgressTick;
						return;
					}
					else
					{
						timerDataPtr->TickProgress = timerDataPtr->TickProgress % tickRateSeconds;
						timerDataPtr->Status = TimerStatus.Running;
					}
				}
				else
				{
					tickRateSeconds = deltaTime;
				}

				int prevLoop = (int)(timerDataPtr->ElapsedTime / timerDuration + math.EPSILON);
				timerDataPtr->ElapsedTime += tickRateSeconds;
				int curLoop = (int)(timerDataPtr->ElapsedTime / timerDuration  + math.EPSILON);
				float newElapsedTime = timerDataPtr->ElapsedTime;
				            
				if(Hint.Unlikely(prevLoop < curLoop)) //complete duration
				{
					float loopCount = timerDataPtr->Loops;
					if(Hint.Unlikely(timerDataPtr->CompletedLoops > loopCount))
					{
						timerDataPtr->ElapsedTime = timerDuration * (loopCount+1);
						if(isPreserverd)
						{
							timerDataPtr->PrevStatus = TimerStatus.Finished;
							timerDataPtr->Status = TimerStatus.Preserved;
						} 
						else
						{
							timerDataPtr->Status = TimerStatus.Finished;
							RemoveTimerIndexs.AddNoResize(dataIndex);
						}
						return;
					}

					timerDataPtr->Status = TimerStatus.NewLoop;
				}
			}
        }
    }
}
