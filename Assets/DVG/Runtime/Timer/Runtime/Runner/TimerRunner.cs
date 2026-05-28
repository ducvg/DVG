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
			if(count <= 0) return;
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

					switch (timerData.Status)
					{
						case TimerStatus.Running:
							timerManagedData.InvokeOnTick(timerData.ElapsedTime);
							continue;
						case TimerStatus.Completed:
							timerManagedData.InvokeOnTick(timerData.ElapsedTime);
							timerManagedData.InvokeOnComplete();
							if(timerData.IsPreserved) timerData.Status = TimerStatus.Preserved;
							continue;
						case TimerStatus.NewLoop:
							timerManagedData.InvokeOnTick(timerData.ElapsedTime);
							timerManagedData.InvokeOnLoopComplete(timerData.CompletedLoops, timerData.ElapsedTime, timerData.LoopElapsedTime);
							timerData.Status = TimerStatus.Running;
							continue;
					}

					if(timerManagedData.bindOwnerCancellationToken.IsCancellationRequested)
					{
						if(timerData.Status == TimerStatus.Disposed) continue;

						timerData.Status = TimerStatus.Disposed;
						timerManagedData.InvokeOnDisposed();
						removeTimerIndexs.AddNoResize(i);
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
