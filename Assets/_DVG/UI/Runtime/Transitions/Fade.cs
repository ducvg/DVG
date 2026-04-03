using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;

#pragma warning disable CS0649, IDE0044 // unassign, readonly suggestion

namespace DVG.UI
{    
    [Serializable]
    public sealed class Fade : ITransition
    {
        [SerializeField] private SerializableMotionSettings<float, NoOptions> _setting;
        [SerializeField] private CanvasGroup _canvasGroup;
        private MotionHandle _handle;

        public UniTask Run<T>(T owner) where T : BaseCanvas
        {
            _handle = LMotion.Create(_setting).BindToAlpha(_canvasGroup);
            return _handle.ToUniTask(owner.destroyCancellationToken);
        }

        public void Complete<T>(T owner) where T : BaseCanvas
        {
            _handle.Complete();
            // _canvasGroup.alpha = _setting.EndValue;
        }
    }
}