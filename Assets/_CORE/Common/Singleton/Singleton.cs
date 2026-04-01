using UnityEngine;

namespace DVG
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (!instance)
                {
                    instance = FindAnyObjectByType<T>();
                }

                return instance;
            }
            protected set => instance = value;
        }

        protected virtual void Awake()
        {
            if (instance && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this as T;
        }
    }

    public abstract class PersistentSingleton<T> : Singleton<T> where T : MonoBehaviour
    {
        protected override void Awake()
        {
            base.Awake();
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
    }
}