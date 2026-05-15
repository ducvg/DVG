using System;
using DVG.StateMachine;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerMovementStateMachine _movementStateMachine;
    
    void Start()
    {
        _movementStateMachine.SetOwner(this);
    }

    void Update()
    {
       if(Input.GetKeyDown(KeyCode.Alpha1))
       {
            _movementStateMachine.ChangeState(_movementStateMachine.IdleState);
       }

       if(Input.GetKeyDown(KeyCode.Alpha2))
       {
            _movementStateMachine.ChangeState(_movementStateMachine.JumpState);
       }

       if(Input.GetKeyDown(KeyCode.Alpha3))
       {
            _movementStateMachine.ChangeState(_movementStateMachine.RunState);
       }
    }
}

[Serializable]
public class PlayerMovementStateMachine : StateMachine<Player>
{
    public readonly IdleState IdleState = new();
    public readonly JumpState JumpState = new();
    [field: SerializeField] public RunState RunState {get; private set;}
}

public class IdleState : State<Player>, IUpdate
{
    public override void OnEnter(Player owner)
    {
        Debug.Log("Enter Idle State");
    }

    public override void OnExit(Player owner)
    {
        Debug.Log("Exit Idle State");
    }

    public void Update(float deltaTime)
    {
        Debug.Log("Update Idle State");
    }
}

public class JumpState : State<Player>, ILateUpdate, IFixedUpdate
{
    public override  void OnEnter(Player owner)
    {
        Debug.Log("Enter Jump State");
    }

    public override void OnExit(Player owner)
    {
        Debug.Log("Exit Jump State");
    }

    public void FixedUpdate(float fixedDeltaTime)
    {
        Debug.Log("FixedUpdate Jump State");
    }

    public void LateUpdate(float deltaTime)
    {   
        Debug.Log("LateUpdate Jump State");
    }

}

[Serializable]
public class RunState : State<Player>, IUpdate, IFixedUpdate
{
    [field: SerializeField] public ParticleSystem SmokeParticle {get; private set;}

    public override void OnEnter(Player owner)
    {
        if(SmokeParticle != null) SmokeParticle.Play();
        Debug.Log("Enter Run State");
    }

    public override void OnExit(Player owner)
    {
        if(SmokeParticle != null) SmokeParticle.Stop();
        Debug.Log("Exit Run State");
    }

    public void FixedUpdate(float fixedDeltaTime)
    {
        Debug.Log("FixedUpdate Run State");
    }

    public void Update(float deltaTime)
    {
        Debug.Log("Update Run State");
    }
}
