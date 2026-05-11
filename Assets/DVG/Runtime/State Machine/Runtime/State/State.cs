using System.Runtime.CompilerServices;
using UnityEngine;

namespace DVG.StateMachine
{
    public interface IState
    {
        public bool IsFinished { get; }
    }
    
    public abstract class State<TOwner> : IState where TOwner : MonoBehaviour
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