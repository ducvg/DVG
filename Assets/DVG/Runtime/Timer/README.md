# Timer
https://github.com/ducvg/DVG.git?path=Assets/DVG/Runtime/Timer
#### Features
 - No allocation with struct based design.
 - Utilizing unity DOTS for optimization.
 - Hooked onto unity internal loops allows precise game timing.
 - **Ticks will run before monobehaviour scripts update's (timer updates before monobehaviour update)**, shouldnt be a problem tho
### Example
```csharp
public class Character: MonoBehaviour
{
	private float hp = 100;
	Timer skillCooldownTimer;

	void Start()
	{
		skillCooldownTimer = Timer.Create(10f, preserve: true).BindTo(this);
	}

	public void UseSkill()
	{
		if(skillCooldownTimer.IsRunning) return;
		skillCooldownTimer.Run();
	}

	public void TakeDotDamage(float damagePerInstance, float duration, float tickInterval)
	{
		Timer.Create(duration, tickInterval).BindTo(this)
			.OnTick(this, (character, elapsedTime) => character.hp -= damagePerInstance)
			.Run();
	}
}
```
## API
#### Settings
- `TimerSetting` hold settings fields
	- `JobBatchCount`: default 32, used for scheduling update timers job. Increase or decrease accordingly to usage.
#### Create Timer
- `Timer.Create(duration)` will return a `Timer` instance. A Timer instance can be used for start, paused, continue, complete and stop.
	- Start: when the timer is created after `.Create` the timer will not run by default but wait until `.Run()`.
	- Paused: when `.Paused()` on a running timer.
	- Continue: when `.Run()` on a paused timer.
	- Complete: when timer complete its duration normally, or skipped by `.Complete()`
	- Stop: when the gameobject binded in timer with `.BindTo()` is destroyed, or by `.Stop()`
- `.Create` parameters:
	- `float duration`: the duration of the timer.
	- `float tickRateSeconds`:default 0, used for `.OnTick` callback, invoke every after that time amount.
	- `int loops`: default 0, timer keep running until completed this amount of loop.
	- `bool preserved`:defaut false, by default timer will be cleaned after it is Stopped or Finished, set to true will allow reuse. **Can cause Leak, use with .BindTo() or .Dipose()**
	- `TimerTiming timing`: default scaled time. `TimerTiming.ScaledTime` or `.UnscaledTime`
	- `TimerUpdater updater`: default Update. `TimerUpdater.EarlyUpdate`, `.FixedUpdate`, `.Update`,...
 - `.BindTo(Monobehaviour)` is optional. It will bind the timer lifetime to a gameobject ensure no leak, destroy the binded gameobject will release the timer.
#### Use Timer instance
- Get info:
  	- `.Duration` return timer duration
  	- `.ElapsedTime` return timer total elapsed time, include loops.
  	- `.Loops` return total loops count
  	- `.CompletedLoops` return completed loops count
  	- `.CycleElapsedTime` return current loop elapsed time
  	- `.IsRunning`, `.IsComplete`, `.IsPaused`
- Actions:
	- `.Run()`: start the timer when its created or **Continue** the timer if its Paused.
	- `.Pause()`: pause the timer when it is Running.
	- `.Stop()`: stop the timer when it is Running or Paused. Retain timer's progress.
	- `.Complete()`: complete the timer immdiately, including its progress.
	- `.Reset()`: reset timer progress back to first creation with `.Create()`;
#### Add Callback
- Chain after a timer instance
	- `.OnStart()`: invoked when the timer first started.
	- `.OnTick(float elapsedTime)`: invoke after every update of the updater or every `tickRateSeconds`
	- `.OnComplete()`: invoked when elapsed time reach duration, or `.Complete()`
	- `.OnPause(float elapsedTime)`: invoked when call `.Pause()` on a running timer.
	- `.OnContinue(float elapsedTime)`: invoked when call `.Run()` on a paused timer.
	- `.OnStop()`: invoked when `.BindTo()` gameobject is destroyed, or by `.Stop()`
	- `.OnLoopComplete(int loop, float totalElapsedTime, float cycleElapseTime)`: invoke when a loop complete.
