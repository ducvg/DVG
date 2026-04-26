using UnityEngine;
  
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindAnyObjectByType<T>();
            }

            return _instance;
        }
        protected set => _instance = value;
    }

    protected virtual void Awake()
    {
        if (_instance && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this as T;
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

public abstract class ScriptableSingleton<T> : ScriptableObject where T : ScriptableObject
{
    public static T Instance {get; private set;}

    protected void Awake()
    {
        if(Instance != null && Instance != this) return;
        Instance = this as T;
    }
}