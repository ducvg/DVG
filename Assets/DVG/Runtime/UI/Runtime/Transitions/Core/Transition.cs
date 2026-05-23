using System.Threading;
using Cysharp.Threading.Tasks;

namespace DVG.UI
{
    public abstract class Transition
    {   
        public abstract UniTask Run(CancellationToken ct);
        public abstract void Complete();
    }
}
