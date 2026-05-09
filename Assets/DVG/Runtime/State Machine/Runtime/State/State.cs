using System.Runtime.CompilerServices;
using UnityEngine;

namespace DVG.StateMachine
{
    public interface IStateStatus
    {
        public bool IsFinished { get; }
    }
    
    public abstract class State<TOwner> : IStateStatus where TOwner : MonoBehaviour
    {
        public bool IsFinished 
        { 
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get; 
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal set; 
        }
        
        public abstract void OnEnter(TOwner owner);
        public abstract void OnExit(TOwner owner);
    }
}