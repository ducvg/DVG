# About
Some systems that i aim for the least abstraction and voodoo magic, each system should be understood just by reading methods' name.<br>
These also aim for minimal uses of monobehaviours/components and no enum switches.

# Install
Add Git package URL: https://github.com/ducvg/DVG.git?path=Assets/DVG<br>
Note: This will auto install all [included packages](#Included) below, individual package links on it's section.<br>
Dependency: UniTask, LitMotion<br>
Odin inspector will override custom editor<br>

# Included
- [UI](#UI)
- [State Machine](#State-Machine)
- [Audio](#Audio)
- [Pool](#Pool)
- [Timer](#Timer)

# UI
Git package URL: https://github.com/ducvg/DVG.git?path=Assets/DVG/Runtime/UI<br>
Add UIManager on a gameObject, this is a DontDestroyOnLoad Singleton wrap around all UI Canvases. Every canvas inherit from class BaseCanvas. <br>
**All Canvas needs to be prefab assigned on UIManager inspector** 

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

# State Machine
Git package URL: https://github.com/ducvg/DVG.git?path=Assets/DVG/Runtime/State%20Machine<br>
Full serializable state machine, mark [Serializable] on any state machines or states that want to be serialized<br>
#### State Update Behaviours
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
The state machine, must implement StateMachine<Owner> with owner is who using the state machine. This serve 2 purposes: cache the states and serialize the states on inspector.
```csharp
[Serializable]
public sealed class PlayerMovementStateMachine : StateMachine<Player>
{
    public readonly PlayerIdleState IdleState = new();
    [field: SerializeField] public readonly PlayerWalkState WalkState { get; private set; }
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

[System.Serializable]
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

# Timer
A modified version from GitAmend, i cleanup some parts, change some name and slight methods behaviour more suitable for me.<br>
**Timers will tick before Monobehaviour's Update().**<br>
Included default timers:
- CountdownTimer: count down from the set duration every tick (-Time.deltaTime)

#### Usage
```csharp
CountdownTimer timer = new(5); //5 secs
timer.OnTimerStart += () => Debug.Log("Start now")
timer.OnTimerStop += () => Debug.Log("Timer stopped/finished")
float elapsedTime = timer.CurrentTime;
bool isTimerRunning = timer.IsRunning;

float newDuration = 10;
timer.SetDuration(newDuration); //count again from 10, doesnt pause
timer.Start();
timer.Stop();
timer.Reset(); //back to 10
timer.Pause();
tiemr.Resume();
```
Create custom timer:
```csharp
public sealed class UpdateTimer : ITimer
{
    public event Action OnUpdate;
    ...
    public override void Tick()
    {
        if(!IsRunning) return;
        if(IsFinished())
        {
            Stop();
            return;
        }
        CurrentTime += Time.deltaTime;
        OnUpdate?.Invoke();
    }

    [MethodImpl]
    public override bool IsFinished() => CurrentTime >= _duration;
}
```
