using System.Runtime.CompilerServices;
using UnityEngine;

namespace DVG.StateMachine
{
    public abstract class State<TOwner> where TOwner : MonoBehaviour
    {
        internal bool IsFinished;
        
        public abstract void OnEnter(TOwner owner);
        public abstract void OnExit(TOwner owner);
    }
}