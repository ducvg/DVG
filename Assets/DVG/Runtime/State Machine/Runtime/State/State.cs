using UnityEngine;

namespace DVG.StateMachine
{
    public interface IStateStatus
    {
        internal bool IsFinished { get; set; }
    }
    
    public abstract class State<TOwner> : IStateStatus where TOwner : MonoBehaviour
    {
        internal bool IStateStatus.IsFinished { get; set; }
        
        public abstract void OnEnter(TOwner owner);
        public abstract void OnExit(TOwner owner);
    }
}