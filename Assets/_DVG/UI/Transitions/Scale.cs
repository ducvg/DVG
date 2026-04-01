using System;
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

        public UniTask Run<T>(T owner) where T : BaseCanvas
        {
            _handle = LMotion.Create(_setting).BindToLocalScale(_target);
            return _handle.ToUniTask(owner.destroyCancellationToken);
        }

        public void Complete<T>(T owner) where T : BaseCanvas
        {
            _handle.Complete();
            // _target.localScale = _setting.EndValue;
        }
    }
}
