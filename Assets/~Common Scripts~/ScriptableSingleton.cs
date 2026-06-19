using UnityEditor;
using UnityEngine;

namespace Zoomer
{
	public abstract class ScriptableSingleton<T> : ScriptableSingleton where T : ScriptableObject
	{
		public static T Instance => _instance;
		private static T _instance;

		protected void Awake()
		{
			if (_instance != null && _instance != this) return;
			_instance = this as T;
		}
	}

	public abstract class ScriptableSingleton : ScriptableObject
	{
		[InitializeOnLoadMethod]
		private static void LoadPreloadedAssets() //fr
		{
			foreach (var asset in PlayerSettings.GetPreloadedAssets())
			{
				asset.GetEntityId(); //random shit to load it
			}
		}

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
}