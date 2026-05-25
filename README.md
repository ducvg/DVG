# About
Some systems that i aim for the least abstraction and voodoo magic, each system should be understood just by reading methods' name.<br>
These also aim for minimal uses of monobehaviours/components and no enum switches.

# Install
https://github.com/ducvg/DVG.git?path=Assets/DVG<br>
Note: This will auto install all [included packages](#Included) below, individual package on it's section.<br>
Dependency: UniTask, LitMotion<br>
Odin inspector will override custom editor if installed<br>

# Module
- [Timer](#Timer)
- [State Machine](#State-Machine)
- [Audio](#Audio)
- [UI](#UI)
- [Pool](#Pool)

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
- `TimerSetting` static class hold configurable settings fields
	- `JobBatchCount`: default 32, used for scheduling update timers job. Increase or decrease accordingly to usage.
#### Create Timer
- Start everything with `Timer.Create(duration)` to get a Timer instance.
- `.Create` overload parameters:
	- `float duration`: the duration of the timer.
	- `float tickRateSeconds`: Time interval between OnTick callbacks and ElapsedTime changes.
	- `int loops`: default 0, timer keep running until completed this amount of loops.
	- `bool preserved`:defaut false, by default timer will be freed after it is Stopped or Finished, set to true will allow reuse.<br> **Will Leak if not use with .BindTo() or .Dispose()**
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

# State Machine
https://github.com/ducvg/DVG.git?path=Assets/DVG/Runtime/State%20Machine<br>
#### Feature
 - Supported updates: EarlyUpdate, FixedUpdate, Update, LateUpdate(PreLateUpdate in internal loop)
 - Implement interface with according updates: IUpdate, IFixedUpdate, ...
 - State updates follows the unity event order: https://docs.unity3d.com/6000.3/Documentation/Manual/execution-order.html
 - **Updates's behaviour will run before monobehaviour scripts update's behaviour (state updates before monobehaviour update)**, shouldnt be a problem tho
#### Example Usage
The owner, the one using the state machine(s):
```csharp
public sealed class Player : MonoBehaviour
{
    [field: SerializeField] public PlayerMovementStateMachine MovementStateMachine  { get; private set; }
    //public PlayerAttackStateMachine AttackStateMachine  { get; private set; }

    private void Awake()
    {
        MovementStateMachine.SetOwner(this); //call once
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            MovementStateMachine.ChangeState(MovementStateMachine.IdleState);
        }
        if(Input.GetKeyDown(KeyCode.W))
        {
            MovementStateMachine.ChangeState(MovementStateMachine.WalkState);
        }
    }
}
```
The state machine, must implement StateMachine&lt;Owner&gt;. This serve 2 purposes: cache the states and serialize the states on inspector.
```csharp
[Serializable]
public sealed class PlayerMovementStateMachine : StateMachine<Player>
{
    [field: SerializeField] public PlayerWalkState WalkState { get; private set; }
    public readonly PlayerIdleState IdleState = new();
}
```
The state, must implement IState<Owner> with owner is who using the state machine.<br>
To have updates within unity internal loops, implement the corresponding interfaces or none if not needed.
```csharp
public sealed class PlayerIdleState : IState<Player>, IEarlyUpdate, IUpdate, ILateUpdate, IFixedUpdate
{
    public void OnEnter(Player owner) => Debug.Log($"{GetType().Name} OnEnter");
    public void OnExit(Player owner) => Debug.Log($"{GetType().Name} OnExit");
    
    public void EarlyUpdate() => Debug.Log($"{GetType().Name} EarlyUpdate");
    public void FixedUpdate() => Debug.Log($"{GetType().Name} FixedUpdate");
    public void Update() => Debug.Log($"{GetType().Name} Update");
    public void LateUpdate() => Debug.Log($"{GetType().Name} LateUpdate");
}

[Serializable]
public sealed class PlayerWalkState : IState<Player>, IUpdate, IFixedUpdate
{
    [SerializeField] private ParticleSystem _trailParticle;
    public void OnEnter(Player owner)
    {
        if(_trailParticle != null) _trailParticle.Play();
        Debug.Log($"{GetType().Name} OnEnter");
    }

    public void OnExit(Player owner)
    {
        if(_trailParticle != null) _trailParticle.Stop();
        Debug.Log($"{GetType().Name} OnExit");
    }

    public void FixedUpdate() => Debug.Log($"{GetType().Name} FixedUpdate");
    public void Update() => Debug.Log($"{GetType().Name} Update");
}
```

# Audio
Git package URL: https://github.com/ducvg/DVG.git?path=Assets/DVG/Runtime/Audio<br>
Add AudioManager on a gameObject, this is a DontDestroyOnLoad Singleton wrap around audio controllers.<br>Every controller implement IAudioController, add custom controller by create class implement this then assign on AudioManager inspector. 2 controllers prepared: SfxAudio and BackgroundAudio
To play a audio, call .Play on a controller and pass in an IAudioData. <br>

#### AudioData included: 
- SingleAudioData: play the audio clip asigned on the inspector.
- RandomAudioData: play a random audio clip in the clip array on the inspector.
- 
#### Usage
Get a controller and play the audio:
```csharp
[SerializeField] private SingleAudioData backgroundAudio; 
[SerializeField] private RandomAudioData shootSfx; 
...
AudioManager.Get<SfxController>.Play(shootSfx);
AudioManager.Get<BackgroundController>.Play(backgroundAudio);
```
Create a AudioData:
```csharp
[System.Serializable]
public sealed class CustomData : IAudioData
{...}
```
Create a AudioController:
```csharp
[System.Serializable]
public sealed class VoiceAudio : IAudioController
{...}
```
then assign the new voice audio controller on the audioManager inspector normally like others.

# UI
Git package URL: https://github.com/ducvg/DVG.git?path=Assets/DVG/Runtime/UI<br>
##### Dependency (auto installed)
- Litmotion
- UniTask
- [Timer](#Timer)
<br>**All Canvas needs to be prefab assigned on UIManager inspector** 
### Transtion
Canvas may or may not have open/close transition, to add transition: select the desired transitions on the canvas's obj inspector<br>
**4 transitions included: Move, Fade, Rotate, Scale.**<br>
Open/close can have multiple transitions that will be combined to create a more complex transitions.
<br>![img.png](GitResource~/img.png) <br>

Create custom transition:
```csharp
[System.Serializable] //must be serializable
public sealed class ParticleTranstion : ITransition
{
    ...

    public async UniTask Run(CancellationToken ct)
    {
        _particle.Play();
        while(_particle.IsRunning()) await UniTask.Yield(ct)
    }
    
    public void Complete()
    {
        _particle.Stop();
    }
}
```

### Open/Close UI
```csharp
UIManager.Open<InventoryCanvas>();
await UIManager.OpenAsync<InventoryCanvas>(); //wait all transition complete
UIManager.OpenImmediate<SettingCanvas>(); //immediately open without any transition;
```
```csharp
UIManager.Close<InventoryCanvas>();
await UIManager.CloseAsync<InventoryCanvas>(); //wait all transition complete
UIManager.CloseImmediate<SettingCanvas>(); //immediately close without any transition;

UIManager.CloseAll(); //same with .Close but for all active canvas
await UIManager.CloseAllAsync();
UIManager.CloseAllImmediate();
```
```csharp
GameplayCanvas gameCanvas = UIManager.GetCanvas<GameplayCanvas>(); //will open if canvas hasnt open before
UIManager.UnloadCanvas<MinigameCanvas>(); //destroy the pooled canvas and release reference to the canvas;
```
```csharp
//Create a canvas, save this as a prefab and drag on UI Manager, no addressable support atm
public sealed class
```

### Pool, Timeout
All canvas are pooled by default, it can be manually cleaned by Destroy(canvas.gameobject) with timeoutStrategy set as Null or be auto with a Timeout Strategy (only InactiveTimout is included atm)<br>
Select a strategy to free up canvas that arent used frequently. The timer will tick (and destroy if finish) before script's Update() and reset time on open before any transition.<br>
Creating custom Timeout Strategy:
```csharp
[System.Serializable]
public sealed class CustomTimeout : ITimeout
{
    [SerializedField] private int _duration;
    Timer timer = new CountdownTimer(); //or coroutine, async await

    public CustomTimeout()
    {
        timer.OnFinish += () => Debug.Log("Timeout");
    }

    public void Run(BaseCanvas owner) 
    {
        timer.SetTime(_duration);
        timer.Start();
    }
    
    public void Stop(BaseCanvas owner)
    {
        timer.Stop();
    }
}
```

# Pool
Git package URL: https://github.com/ducvg/DVG.git?path=Assets/DVG/Runtime/Pool<br>
Object pooling is used to avoid the overhead of creating/destroying object instances and C# Garbage Collector kicking in. Unity already provide a `ObjectPool<T>` for use. However i make my own for the sole purpose of avoid using delegate as it can allocate more and can have unwanted hidden reference. If want to run something right after getting object or return the instance then just call it's method from the factory or something.<br>
This version use Stack as the main holders for minimal ram usage (out of all collection: inner array and a tail index).<br>
3 default pools are provided:
- `ComponentPool<T> where T : UnityEngine.Componenet`: auto .SetActive(true) on rent from pool and (false) when return;
- `UnityPool<T> where T : UnityEngine.Object`: same as ComponentPool but doesnt enable or disable (for like materials, clips,...)
- `ObjectPool<T>`: Just getting and return normal c# classes. unity.Object can leak (not destroy on max capacity) on return with this.
#### Usage
```csharp
IObjectPool<Bullet> bulletPool = new ComponentPool(prefab, defaultSize, maxCapacity) 
IObjectPool<Bullet> bulletPool = new ComponentPool(prefab, transformParent, defaultSize, maxCapacity) 

Bullet bullet = bulletPool.Rent();
bulletPool.Return(bullet);
```
Custom pool:
```csharp
public sealed class CustomPool<T> : IObjectPool<T>
{...}
```
