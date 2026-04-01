using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace DVG.Pool
{
    //reuse reference
    public sealed class UnityPool<T> : IObjectPool<T> where T : Object
    {
        private readonly T _prefab;
        private readonly Stack<T> _inactiveStack;
        private readonly int _maxSize;
        
        public UnityPool(T prefab, int defaultCapacity = 10, int maxSize = 100)
        {
            _prefab = prefab;
            _maxSize = maxSize;
            _inactiveStack = new(defaultCapacity);
        }

        public void Preload(int amount)
        {
            for (int i = 0; i < amount; ++i)
            {
                T instance = Create();
                _inactiveStack.Push(instance);
            }
        }

        private T Create()
        {
            return Object.Instantiate(_prefab);
        }

        public T Rent()
        {
            T instance;

            if (_inactiveStack.Count == 0) instance = Create();
            else instance = _inactiveStack.Pop();
            return instance;
        }

        public void Return(T instance)
        {
            if (_inactiveStack.Count > _maxSize)
            {
                Object.Destroy(instance);
                return;
            }

            _inactiveStack.Push(instance);
        }

        public void Clear()
        {
            while (_inactiveStack.Count > 0)
            {
                Object.Destroy(_inactiveStack.Pop());
            }
            
            _inactiveStack.Clear();
        }
    }
}