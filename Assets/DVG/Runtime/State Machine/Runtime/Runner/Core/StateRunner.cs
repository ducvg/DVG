using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DVG.StateMachine.Editor;
using UnityEngine;

namespace DVG.StateMachine
{
    internal interface IStateRunner
    {
        public void Run(float deltaTime, float fixedDeltaTime);
        public bool Register<TOwner>(State<TOwner> state) where TOwner : MonoBehaviour;
        // public void Unregister<TOwner>(State<TOwner> state) where TOwner : MonoBehaviour;
    }
    
    internal abstract class StateRunner<TUpdate> : IStateRunner where TUpdate : IState
    {
        private const int _initActiveSize = 128;
        private const int _initPendingAddSize = 64;
        
        protected TUpdate[] _states = new TUpdate[_initActiveSize];
        protected int _tailIndex;
        protected LiteStack<TUpdate> _pendingAddStack = new(_initPendingAddSize);

        public abstract void Run(float deltaTime, float fixedDeltaTime);

        public bool Register<TOwner>(State<TOwner> state) where TOwner : MonoBehaviour
        {
            if (state is not TUpdate u) return false;
            
            _pendingAddStack.Push(u);
            return true;
        }
    }
}