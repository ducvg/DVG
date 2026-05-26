using Unity.Collections;
using Unity.Jobs;

namespace DVG.Timers
{
    public sealed partial class TimerRunner
    {
		internal readonly TimerDataStorage DataStorage;

		internal TimerRunner(TimerDataStorage dataStorage)
		{
			DataStorage = dataStorage;
		}

        internal unsafe void Update(float scaledDeltaTime, float unscaledDeltaTime)
        {
			var count = DataStorage.Count;
			using NativeList<int> removeTimerIndexs = new NativeList<int>(count, Allocator.TempJob);
			fixed (TimerData* dataPtr = DataStorage.GetDataSpan())
			{
				new UpdateTimerJob
				{
					ScaledDeltaTime = scaledDeltaTime,
					UnscaledDeltaTime = unscaledDeltaTime,
					RemoveTimerIndexs = removeTimerIndexs.AsParallelWriter(),
					DataArrayPtr = dataPtr,
				}
				.Schedule(count, TimerSettings.JobBatchCount).Complete();

				var managedDataSpan = DataStorage.GetManagedDataSpan();
                for (int i = 0; i < count; ++i)
				{
					ref var timerData = ref *(dataPtr + i);
					var timerManagedData = managedDataSpan[i];

					if(timerManagedData.bindOwnerCancellationToken.IsCancellationRequested)
					{
						if(timerData.Status == TimerStatus.Disposed) continue;

						timerData.Status = TimerStatus.Disposed;
						timerManagedData.OnDisposed();
						removeTimerIndexs.AddNoResize(i);
						continue;
					}

					switch (timerData.Status)
					{
						case TimerStatus.Running:
							timerManagedData.OnTick(timerData.ElapsedTime);
							continue;
						case TimerStatus.Completed:
							timerManagedData.OnTick(timerData.ElapsedTime);
							timerManagedData.OnComplete();
							if(timerData.IsPreserved) timerData.Status = TimerStatus.Preserved;
							continue;
						case TimerStatus.NewLoop:
							timerManagedData.OnTick(timerData.ElapsedTime);
							timerManagedData.OnLoopComplete(timerData.CompletedLoops, timerData.ElapsedTime, timerData.LoopElapsedTime);
							timerData.Status = TimerStatus.Running;
							continue;
					}
				}
			}
			
			DataStorage.RemoveAll(removeTimerIndexs);
        }

		internal void Dispose()
		{
			DataStorage.Clear();
		}
    }
}
