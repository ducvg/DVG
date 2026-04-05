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

        public UniTask Open(CancellationToken ct)
        {
            int count = openTransitions.Length;
            if (count == 0) return UniTask.CompletedTask;
            
            var tasks = ArrayPool<UniTask>.Shared.Rent(count);
            for (int i = 0; i < count; ++i)
            {
                tasks[i] = openTransitions[i].Run(ct);
            }
            return UniTask.WhenAll(tasks);
        }

        public void CompleteOpen()
        {
            foreach (var transition in openTransitions)
            {
                transition.Complete();
            }
        }

        public UniTask Close(CancellationToken ct)
        {
            int count = closeTransitions.Length;
            if (count == 0) return UniTask.CompletedTask;
            
            var tasks = ArrayPool<UniTask>.Shared.Rent(count);
            for (int i = 0; i < count; ++i)
            {
                tasks[i] = closeTransitions[i].Run(ct);
            }
            return UniTask.WhenAll(tasks);
        }

        public void CompleteClose()
        {
            foreach (var transition in closeTransitions)
            {
                transition.Complete();
            }
        }
    }
}

