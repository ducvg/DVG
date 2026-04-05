using UnityEngine;

namespace DVG.Pool
{
    public interface IObjectPool<T>
    {
        public void Preload(int amount);
        public T Rent();
        public void Return(T instance);
        public void Clear();
    }
}