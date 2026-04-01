using UnityEngine;

namespace DVG
{
    public abstract class ScriptableSingleton<T> : ScriptableObject where T : ScriptableObject
    {
        private static T _instance;
        public static T Instance => _instance;

        protected virtual void OnEnable()
        {
            if(_instance != null && _instance != this)
            {
                Debug.LogError($"Singleton already exists: {typeof(T).Name}\npress to navigate", _instance);
                return;
            }
            _instance = this as T;
        }
    }
}
