using UnityEngine;

namespace DVG.StateMachine
{
    //TOwner is the MonoBehaviour that owns the state machine that owns the state
    public interface IState<TOwner> where TOwner : MonoBehaviour
    {
        public void OnEnter(TOwner owner);
        public void OnExit(TOwner owner);
    }
}
