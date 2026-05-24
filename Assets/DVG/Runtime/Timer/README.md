# Timer
https://github.com/ducvg/DVG.git?path=Assets/DVG/Runtime/Timer
#### Features
 - No allocation with struct based design.
 - Utilizing unity DOTS for performance.
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
			.OnTick(this, (character,_) => character.hp -= damagePerInstance)
			.Run();
	}
}
```
## API
#### Settings
- `TimerSetting` hold settings fields
	- `JobBatchCount`: default 32, used for scheduling update timers job. Increase or decrease accordingly to usage.
#### Create Timer
- Start everything with `Timer.Create(duration)` to get a Timer instance.
- `.Create` overload parameters:
	- `float duration`: the duration of the timer.
	- `float tickRateSeconds`: Time interval between OnTick callbacks and ElapsedTime changes.
	- `int loops`: default 0, timer keep running until completed this amount of loops.
	- `bool preserved`:defaut false, by default timer will be cleaned after it is Stopped or Finished, set to true will allow reuse.<br> **Will Leak if not use with .BindTo() or .Dispose()**
	- `TimerTiming timing`: default scaled time. `TimerTiming.ScaledTime` or `.UnscaledTime`
	- `TimerUpdater updater`: default Update. `TimerUpdater.EarlyUpdate`, `.FixedUpdate`, `.Update`,...
 - `.BindTo(Monobehaviour)` Optional. this will bind the timer lifetime to a gameobject ensure no leak, destroy the binded gameobject will release the timer and invoke OnDispose() callback.
#### Use Timer instance
- Get info:
  	- `.Duration` return timer duration
  	- `.ElapsedTime` return timer total elapsed time, include loops.
  	- `.Loops` return total loops count
  	- `.CompletedLoops` return completed loops count
  	- `.LoopElapsedTime` return current loop elapsed time
  	- `.IsRunning`, `.IsComplete`, `.IsPaused`
- Actions:
	- `.Run()`: start and invoke OnStart() callback when timer first Created or continue and invoke OnContinue() callback if Paused.
	- `.Pause()`: pause the timer when it is Running, invoke OnPause() callback.
	- `.Complete()`: complete the timer progress immediately, invoke OnComplete() callback.
	- `.ResetTime()`: reset timer progress, wont stop running if already running`.
 	- `.ResetTime(float duration)`: above but with a new duration.
  	- `.Reset()`: reset timer progress, need start manually again and OnStart() callback will be invoked.
  	- `.Reset(float duration)`: above but with a new duration.
  	- `.Dispose()`: ensure the timer is freed. Made for preserved timer;
#### Add Callback
all callback have 2 version, with and without context for closure 
```csharp
public class MrBreast
{
	int value;
	public void Method()
	{
		Timer.Create(5f).OnTick((elapsed) => value = elapsed); //allocation for capturing "value" field
		Timer.Create(5f).OnTick(this, (guy, elapsed) => guy.value = elapsed) //no additional allocation
	}
}
```
- Chain after a timer instance
	- `.OnStart()`: invoked when the timer first run.
	- `.OnTick(float elapsedTime)`: invoke after every update of the updater or every `tickRateSeconds`
	- `.OnComplete()`: invoked when elapsed time reach duration, or `.Complete()`
	- `.OnPause(float elapsedTime)`: invoked when call `.Pause()` on a running timer.
	- `.OnContinue(float elapsedTime)`: invoked when call `.Run()` on a paused timer.
	- `.OnLoopComplete(int loop, float totalElapsedTime, float cycleElapseTime)`: invoke when a loop complete.
