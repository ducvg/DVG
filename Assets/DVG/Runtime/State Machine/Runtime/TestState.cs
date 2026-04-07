using System;
using UnityEngine;
//
namespace DVG.StateMachine
{
    public sealed class Player : MonoBehaviour
    {
        [SerializeField] private PlayerMovementStateMachine _movementStateMachine;
    }

    [Serializable]
    public sealed class PlayerMovementStateMachine : StateMachine<Player>
    {
        [field: SerializeField] public PlayerIdleState IdleState { get; private set; }
    }

    public sealed class PlayerIdleState : State<Player>
    {
        [SerializeField] private string _name;

        public void OnEnter(Player owner) { }
        public void OnExit(Player owner) { }
        public void Update(Player owner) { }
        public void LateUpdate(Player owner) { }
        public void FixedUpdate(Player owner) { }
    }
}