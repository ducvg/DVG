# Timer
Git package URL: https://github.com/ducvg/DVG.git?path=Assets/DVG/Runtime/Timer<br>
#### Features
 - No allocation with struct based design.
 - Optimized by utilizing unity DOTS.
 - Hooked onto unity internal loops allows precise game timing.
 - **Ticks will run before monobehaviour scripts update's (timer updates before monobehaviour update)**, shouldnt be a problem tho
#### Example
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
### Usage
- To create a timer, start with `Timer.Create(duration)` this will return a `Timer` instance. A Timer instance can be used for start, paused, continue, complete and stop.
	- Start: when the timer is created after `.Create` the timer will not run by default but wait until `.Run()`.
	- Paused: when `.Paused()` on a running timer.
	- Continue: when `.Run()` on a paused timer.
	- Complete: when timer complete its duration normally, or skipped by `.Complete()`
	- Stop: when the gameobject binded in timer with `.BindTo()` is destroyed, or by `.Stop()`
- `.Create` parameters:
	- `float duration`: the duration of the timer.
	- `float tickRateSeconds`: used for `.OnTick` callback, invoke every after that time amount.
	- `int loops`: default 0, timer keep running until completed this amount of loop.
	- `bool preserved`:defaut false, by default timer will be cleaned after it is Stopped or Finished. setting true will allow reuse. **Can cause Leak, use with .BindTo() or .Dipose()**
	- `TimerTiming timing`: default scaled time. `TimerTiming.ScaledTime` or `.UnscaledTime`
	- `TimerUpdater updater`: default Update. `TimerUpdater.EarlyUpdate`, `.FixedUpdate`, `.Update`,...
- All callbacks have 2 version, with context and without. Example:
```
public class Guy
{
	int value;
	public void Method()
	{
		Timer.Create(5f).OnTick((elapsed) => value = elapsed); //value is captured, cause additional allocation
		Timer.Create(5f).OnTick(this, (guy, elapsed) => guy.value = elapsed) //no additional allocation
	}
}
```
- Callbacks: chain after a timer instance
	- `.OnStart()`: invoked when the timer first started only.
	- `.OnTick(float elapsedTime)`: invoke after every update of the updater or every `tickRateSeconds`
	- `.OnComplete()`: invoked when elapsed time reach duration, or `.Complete()`
	- `.OnPause(float elapsedTime)`: invoked when call `.Pause()` on a running timer.
	- `.OnContinue(float elapsedTime)`: invoked when call `.Run()` on a paused timer.
	- `.OnStop()`: invoked when `.BindTo()` gameobject is destroyed, or by `.Stop()`
	- `.OnLoopComplete(int loop, float totalElapsedTime, float loopElapseTime)`: invoke when a loop complete.
