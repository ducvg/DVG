using UnityEngine;

namespace DVG.StateMachine
{
    public interface IState<TOwner> where TOwner : MonoBehaviour
    {
        public void OnEnter(TOwner owner);
        public void OnExit(TOwner owner);
    }
}
