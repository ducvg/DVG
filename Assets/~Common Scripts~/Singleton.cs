using System;
using UnityEditor;
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

public abstract class ScriptableSingleton<T> : ScriptableSingleton where T : ScriptableObject
{
	public static T Instance => _instance;
	private static T _instance;

	protected virtual void OnEnable()
	{
		if (_instance != null && _instance != this) return;
		_instance = this as T;
	}
}

public abstract class ScriptableSingleton : ScriptableObject
{
	[InitializeOnLoadMethod]
	private static void LoadPreloadedAssets() => PlayerSettings.GetPreloadedAssets();

	private class ScriptableSingletonImporter : AssetPostprocessor
	{
		private static void OnPostprocessAllAssets(
			string[] importedAssets,
			string[] deletedAssets,
			string[] movedAssets,
			string[] movedFromAssetPaths,
			bool didDomainReload)
		{
			bool isDeleteting = deletedAssets.Length > 0;
			bool isMoving = movedAssets.Length > 0 || movedFromAssetPaths.Length > 0;
			if (didDomainReload || isDeleteting || isMoving) return;

			string createdPath = importedAssets[0];
			var singleton = AssetDatabase.LoadAssetAtPath<ScriptableSingleton>(createdPath);
			if (!singleton) return;

			//preload assets preload in runtime, not in editor startup, dumbshit
			var preloadAssetArray = PlayerSettings.GetPreloadedAssets();
			if (preloadAssetArray == null) return;
			foreach (var preloaded in preloadAssetArray)
			{
				if (preloaded.GetType() == singleton.GetType())
				{
					AssetDatabase.DeleteAsset(createdPath);
					EditorGUIUtility.PingObject(preloaded);
					return;
				}
			}
			Array.Resize(ref preloadAssetArray, preloadAssetArray.Length + 1);
			preloadAssetArray[^1] = singleton;
			PlayerSettings.SetPreloadedAssets(preloadAssetArray);
		}
	}
}
