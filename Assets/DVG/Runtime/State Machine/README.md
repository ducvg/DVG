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
