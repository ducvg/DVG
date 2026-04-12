using System;
using UnityEngine;

namespace DVG.StateMachine
{
    public sealed class Player : MonoBehaviour
    {
        [field: SerializeField] public PlayerMovementStateMachine MovementStateMachine  { get; private set; }
        // [field: SerializeField] public PlayerAttackStateMachine AttackStateMachine  { get; private set; }

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

    [Serializable]
    public sealed class PlayerMovementStateMachine : StateMachine<Player>
    {
        [field: SerializeField] public PlayerIdleState IdleState { get; private set; }
        /*serialize unforced*/ public PlayerWalkState WalkState { get; private set; } = new();
    }

    [Serializable]
    public sealed class PlayerIdleState : IState<Player>, IUpdate, IEarlyUpdate, ILateUpdate, IFixedUpdate
    {
        [SerializeField] private string _name;

        public void OnEnter(Player owner)
        {
            Debug.Log($"{GetType().Name} OnEnter");
        }

        public void OnExit(Player owner)
        {
            Debug.Log($"{GetType().Name} OnExit");
        }

        public void Update()
        {
            Debug.Log($"{GetType().Name} Update");
        }

        public void EarlyUpdate()
        {
            Debug.Log($"{GetType().Name} EarlyUpdate");
        }

        public void LateUpdate()
        {
            Debug.Log($"{GetType().Name} LateUpdate");
        }

        public void FixedUpdate()
        {
            Debug.Log($"{GetType().Name} FixedUpdate");
        }
    }
    
    //dont need serializable
    public sealed class PlayerWalkState : IState<Player>, IUpdate, IFixedUpdate
    {
        public void OnEnter(Player owner)
        {
            Debug.Log($"{GetType().Name} OnEnter");
        }

        public void OnExit(Player owner)
        {
            Debug.Log($"{GetType().Name} OnExit");
        }

        public void Update()
        {
            Debug.Log($"{GetType().Name} Update");
        }

        public void FixedUpdate()
        {
            Debug.Log($"{GetType().Name} FixedUpdate");
        }
    }
}