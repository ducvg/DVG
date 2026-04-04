# Audio System

# Install
Add Git package URL: https://github.com/ducvg/DVG.git?path=Assets/_DVG/UI<br>
Auto install dependencies: UniTask, LitMotion (for included transitions), DVG.Common (Timer for timeout)<br>
Works with odin inspector

# Usage
Add UIManager on a gameObject, this is a DontDestroyOnLoad Singleton wrap around all UI Canvases. Every canvas inherit from class BaseCanvas. <br>
**All Canvas needs to be prefab assigned on UIManager inspector** 

### Transtion
Canvas may or may not have open/close transition, to add transition: select the desired transitions on the canvas's obj inspector
**4 Transition included: Move, Fade, Rotate, Scale.**
<br>![img.png](../../../GitResource~/img.png) <br>
Multiple transitions will run same time which can be combined for various design desires.<br>
Creating custom transition:
```csharp
[System.Serializable]
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

### API
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

### Pool, Timeout
All canvas are pooled by default, it can be manally .Destroy(canvas.gameobject) with timeoutStrategy as Null or be auto with a Timeout Strategy (only InactiveTimout is included atm)<br>
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