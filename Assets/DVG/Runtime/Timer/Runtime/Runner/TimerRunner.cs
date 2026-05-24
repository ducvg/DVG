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
					ref TimerData timerData = ref *(dataPtr + i);
					var timerManagedData = managedDataSpan[i];

					if(timerManagedData.bindOwnerCancellationToken.IsCancellationRequested)
					{
						timerData.Status = TimerStatus.Disposed;
						continue;
					}

					switch (timerData.Status)
					{
						case TimerStatus.Running:
							timerManagedData.OnTick(timerData.ElapsedTime);
							continue;
						case TimerStatus.Completed:
							if(timerData.PrevStatus != TimerStatus.Completed) timerManagedData.OnComplete();
							continue;
						case TimerStatus.NewLoop:
							timerManagedData.OnTick(timerData.ElapsedTime);
							timerManagedData.OnLoopComplete(timerData.CompletedLoops, timerData.ElapsedTime, timerData.LoopElapsedTime);
							timerData.Status = TimerStatus.Running;
							continue;
						case TimerStatus.Preserved:
							if(timerData.PrevStatus is TimerStatus.Completed)
							{
								timerManagedData.OnComplete();
								timerData.PrevStatus = TimerStatus.Disposed;
							}
							continue;
						case TimerStatus.Disposed:
							timerManagedData.OnDisposed();
							continue;
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
				ref TimerData timerData = ref *timerDataPtr;
				
				float deltaTime = timerData.Timing == TimerTiming.ScaleTime ? ScaledDeltaTime : UnscaledDeltaTime;
				float timerTickRate = timerData.TickRateSeconds;

				if(Hint.Unlikely(timerData.Status == TimerStatus.Disposed))
				{
					RemoveTimerIndexs.AddNoResize(dataIndex);
					return;
				}
				if(Hint.Unlikely(timerData.Status == TimerStatus.Completed))
				{
					timerData.PrevStatus = TimerStatus.Completed;
					HandleCompleteTimer(ref timerData, dataIndex);
					return;
				}
				if(timerData.Status is TimerStatus.Created or TimerStatus.Paused or TimerStatus.Preserved) return;

				if(Hint.Unlikely(timerData.Duration <= 0))
				{
					HandleCompleteTimer(ref timerData, dataIndex);
					return;
				}

				//custom tick rate handle
				if(timerTickRate > math.EPSILON)
				{
					timerData.TickProgress += deltaTime;
					if(Hint.Likely(timerData.TickProgress < timerTickRate)) return;
					timerData.TickProgress = timerData.TickProgress % timerTickRate;
				}
				else
				{
					timerTickRate = deltaTime;
				}

				//loop handle
				int prevLoop = timerData.CompletedLoops;
				timerData.ElapsedTime += timerTickRate;
				int curLoop = timerData.CompletedLoops;
				            
				if(Hint.Unlikely(prevLoop < curLoop))
				{
					if(timerData.CompletedLoops > timerData.Loops)
					{
						HandleCompleteTimer(ref timerData, dataIndex);
						return;
					}
					timerData.Status = TimerStatus.NewLoop;
					return;
				}
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private void HandleCompleteTimer(ref TimerData timerData, int dataIndex)
			{
				timerData.ElapsedTime = math.max(0, timerData.Duration * (timerData.Loops + 1));
				if(timerData.IsPreserved)
				{
					timerData.PrevStatus = TimerStatus.Completed;
					timerData.Status = TimerStatus.Preserved;
				} 
				else
				{
					timerData.Status = TimerStatus.Completed;
					RemoveTimerIndexs.AddNoResize(dataIndex);
				}
			}
        }
    }
}
