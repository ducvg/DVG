using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
#pragma warning disable CS0649, IDE0044 // unassign, readonly suggestion

namespace DVG.UI
{    
    [Serializable]
    public sealed class Scale : ITransition
    {
        [SerializeField] private SerializableMotionSettings<Vector3, NoOptions> _setting;
        [SerializeField] private Transform _target;
        private MotionHandle _handle;

        public UniTask Run(CancellationToken ct)
        {
            _handle = LMotion.Create(_setting).BindToLocalScale(_target);
            return _handle.ToUniTask(ct);
        }

        public void Complete()
        {
            _handle.Complete();
            // _target.localScale = _setting.EndValue;
        }
    }
}
