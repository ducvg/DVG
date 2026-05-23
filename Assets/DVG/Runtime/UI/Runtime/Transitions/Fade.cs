using System;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;

#pragma warning disable CS0649, IDE0044 // unassign, readonly suggestion

namespace DVG.UI
{    
    [Serializable]
    public sealed class Fade : Transition
    {
        [SerializeField] private SerializableMotionSettings<float, NoOptions> _setting;
        [SerializeField] private CanvasGroup _canvasGroup;
        private MotionHandle _handle;

        public override UniTask Run(CancellationToken ct)
        {
            _handle = LMotion.Create(_setting).BindToAlpha(_canvasGroup);
            return _handle.ToUniTask(ct);
        }

        public override void Complete() 
        {
            _handle.Complete();
            // _canvasGroup.alpha = _setting.EndValue;
        }
    }
}