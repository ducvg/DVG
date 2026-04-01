using System;
using System.Buffers;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using UnityEngine;
#pragma warning disable CS0649, IDE0044 // unassign, readonly suggestion

namespace DVG.UI
{
    [Serializable]
    public sealed class TransitionData
    {
        [SerializeReference] private ITransition[] openTransitions;
        [SerializeReference] private ITransition[] closeTransitions;

        public UniTask Open<T>(T owner) where T : BaseCanvas
        {
            int count = openTransitions.Length;
            var tasks = ArrayPool<UniTask>.Shared.Rent(count);
            for (int i = 0; i < count; i++)
            {
                tasks[i] = openTransitions[i].Run(owner);
            }
            return UniTask.WhenAll(tasks);
        }

        public void CompleteOpen<T>(T owner) where T : BaseCanvas
        {
            foreach (var transition in openTransitions)
            {
                transition.Complete(owner);
            }
        }

        public UniTask Close<T>(T owner) where T : BaseCanvas
        {
            int count = closeTransitions.Length;
            var tasks = ArrayPool<UniTask>.Shared.Rent(count);
            for (int i = 0; i < count; i++)
            {
                tasks[i] = closeTransitions[i].Run(owner);
            }
            return UniTask.WhenAll(tasks);
        }

        public void CompleteClose<T>(T owner) where T : BaseCanvas
        {
            foreach (var transition in closeTransitions)
            {
                transition.Complete(owner);
            }
        }
    }
}

