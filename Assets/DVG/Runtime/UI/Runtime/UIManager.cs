using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace DVG.UI
{
    public sealed class UIManager : MonoBehaviour
    {
        //TODO: addressable update
        [SerializeField] private BaseCanvas[] _prefabList;
        private Dictionary<Type, BaseCanvas> _canvasPrefabs, _loadedCanvases;
        
        private static UIManager _instance;

        void Awake()
        {
            _instance = this;

            int count = _prefabList.Length;
            _canvasPrefabs = new(count);
            _loadedCanvases = new(count);

            foreach (var canvas in _prefabList)
            {
                _canvasPrefabs[canvas.GetType()] = canvas;
            }

            _prefabList = null;
        }

        public static T Open<T>() where T : BaseCanvas
        {
            T canvas = GetCanvas<T>();
        
            canvas.OpenAsync().Forget();
        
            return canvas;
        }
        
        public static async UniTask<T> OpenAsync<T>() where T : BaseCanvas
        {
            T canvas = GetCanvas<T>();
            await canvas.OpenAsync();
            return canvas;
        }

        public static T OpenImmediate<T>() where T : BaseCanvas
        {
            T canvas = GetCanvas<T>();
            canvas.OpenImmediate();
            return canvas;
        }

        public static void Close<T>() where T : BaseCanvas
        {
            if (IsOpened<T>())
            {
                _instance._loadedCanvases[typeof(T)].CloseAsync().Forget();
            }
        }
        
        public static async UniTask CloseAsync<T>() where T : BaseCanvas
        {
            if (IsOpened<T>())
            {
                await _instance._loadedCanvases[typeof(T)].CloseAsync();
            }
        }

        public static void CloseImmediate<T>() where T : BaseCanvas
        {
            if (IsOpened<T>())
            {
                _instance._loadedCanvases[typeof(T)].CloseImmediate();
            }
        }

        public static void CloseAll()
        {
            foreach (var canvas in _instance._loadedCanvases)
            {
                if (canvas.Value != null && canvas.Value.gameObject.activeSelf)
                {
                    canvas.Value.CloseAsync().Forget();
                }
            }
        }

        public static async UniTask CloseAllAsync()
        {
            List<UniTask> tasks = new();
            foreach (var canvas in _instance._loadedCanvases)
            {
                if (canvas.Value != null && canvas.Value.gameObject.activeSelf)
                {
                    tasks.Add(canvas.Value.CloseAsync());
                }
            }
            await UniTask.WhenAll(tasks);
        }

        public static void CloseAllImmediate()
        {
            foreach (var canvas in _instance._loadedCanvases)
            {
                if (canvas.Value != null && canvas.Value.gameObject.activeSelf)
                {
                    canvas.Value.CloseImmediate();
                }
            }
        }

        public static T GetCanvas<T>() where T : BaseCanvas
        {
            if (!IsLoaded<T>())
            {
                T canvas = Instantiate(GetCanvasPrefab<T>(), _instance.transform);
                _instance._loadedCanvases[typeof(T)] = canvas;
            }

            return _instance._loadedCanvases[typeof(T)] as T;
        }

        public static void UnloadCanvas<T>(T canvas) where T : BaseCanvas
        {
            if (IsLoaded<T>())
            {
                Destroy(canvas);
                _instance._loadedCanvases.Remove(typeof(T));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsLoaded<T>() where T : BaseCanvas
        {
            bool isLoaded = _instance._loadedCanvases.ContainsKey(typeof(T));
            if(!isLoaded) return false;
            
            bool isDestroyed = _instance._loadedCanvases[typeof(T)] == null;
            if(isDestroyed) return false;

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsOpened<T>() where T : BaseCanvas
        {
            if (!IsLoaded<T>()) return false;
            if (_instance._loadedCanvases[typeof(T)].IsOpen()) return true;

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static T GetCanvasPrefab<T>() where T : BaseCanvas
        {
            return _instance._canvasPrefabs[typeof(T)] as T;
        }
    }
}