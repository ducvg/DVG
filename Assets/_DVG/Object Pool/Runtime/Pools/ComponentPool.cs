using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DVG.Pool
{
    //enable disable unity components
    public sealed class ComponentPool<T> : IObjectPool<T> where T : Component
    {
        private readonly T _prefab;
        private readonly Stack<T> _inactiveStack;
        private readonly int _maxSize;
        private readonly Transform _parent;

        public ComponentPool(T prefab, Transform parent = null, int defaultCapacity = 10, int maxSize = 100)
        {
            _prefab = prefab;
            _parent = parent;
            _maxSize = maxSize;
            _inactiveStack = new(defaultCapacity);
        }

        public void Preload(int amount)
        {
            for (int i = 0; i < amount; ++i)
            {
                T instance = Create();
                instance.gameObject.SetActive(false);
                _inactiveStack.Push(instance);
            }
        }

        private T Create()
        {
            T instance;
            if (_parent)
            {
                instance = Object.Instantiate(_prefab, _parent);
            }
            else
            {
                instance = Object.Instantiate(_prefab);
            }

            return instance;

        }

        public T Rent()
        {
            T instance;

            if (_inactiveStack.Count == 0) instance = Create();
            else instance = _inactiveStack.Pop();

            instance.gameObject.SetActive(true);

            // instance.OnPoolRent();
            return instance;
        }

        public void Return(T instance)
        {
            // instance.OnPoolReturn();
            if (_inactiveStack.Count > _maxSize)
            {
                Object.Destroy(instance);
                return;
            }

            instance.gameObject.SetActive(false);
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