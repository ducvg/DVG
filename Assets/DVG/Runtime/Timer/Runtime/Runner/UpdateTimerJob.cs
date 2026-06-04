using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Burst.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;

namespace DVG.Timers
{
    public sealed partial class TimerRunner
    {
		[BurstCompile]
		private unsafe struct UpdateTimerJob : IJobParallelFor
        {
			[ReadOnly] public float ScaledDeltaTime, UnscaledDeltaTime;
			[WriteOnly] public NativeList<int>.ParallelWriter RemoveTimerIndexs;
			[NativeDisableUnsafePtrRestriction] public TimerData* DataArrayPtr;

            public void Execute([AssumeRange(0, int.MaxValue)] int dataIndex)
            {
				ref TimerData timerData = ref *(DataArrayPtr + dataIndex);
				
				float deltaTime = timerData.Timing == TimerTiming.ScaleTime ? ScaledDeltaTime : UnscaledDeltaTime;
				float timerTickRate = timerData.TickRateSeconds;

				if(Hint.Unlikely(timerData.Status == TimerStatus.Disposed))
				{
					RemoveTimerIndexs.AddNoResize(dataIndex);
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
				timerData.Status = TimerStatus.Completed;
				if(!timerData.IsPreserved) RemoveTimerIndexs.AddNoResize(dataIndex);
			}
        }
    }
}
