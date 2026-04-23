using System;
using DVG.StateMachine;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerMovementStateMachine _movementStateMachine;

    

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

public class IdleState : IState<Player>, IUpdate
{
    public void OnEnter(Player owner)
    {
        Debug.Log("Enter Idle State");
    }

    public void OnExit(Player owner)
    {
        Debug.Log("Exit Idle State");
    }

    public void Update()
    {
        Debug.Log("Update Idle State");
    }
}

public class JumpState : IState<Player>, ILateUpdate, IFixedUpdate, IUpdate
{
    public void OnEnter(Player owner)
    {
        Debug.Log("Enter Jump State");
    }

    public void OnExit(Player owner)
    {
        Debug.Log("Exit Jump State");
    }

    void IFixedUpdate.FixedUpdate()
    {
        Debug.Log("FixedUpdate Jump State");
    }

    void ILateUpdate.LateUpdate()
    {   
        Debug.Log("LateUpdate Jump State");
    }

    void IUpdate.Update()
    {
        Debug.Log("Update Jump State");
    }
}

[Serializable]
public class RunState : IState<Player>, IUpdate, IFixedUpdate
{
    [field: SerializeField] public ParticleSystem SmokeParticle {get; private set;}

    public void OnEnter(Player owner)
    {
        if(SmokeParticle != null) SmokeParticle.Play();
        Debug.Log("Enter Run State");
    }

    public void OnExit(Player owner)
    {
        if(SmokeParticle != null) SmokeParticle.Stop();
        Debug.Log("Exit Run State");
    }

    public void FixedUpdate()
    {
        Debug.Log("FixedUpdate Run State");
    }

    public void Update()
    {
        Debug.Log("Update Run State");
    }
}
