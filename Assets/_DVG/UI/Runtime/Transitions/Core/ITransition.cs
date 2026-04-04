using System.Threading;
using Cysharp.Threading.Tasks;

namespace DVG.UI
{
    public interface ITransition
    {   
        public UniTask Run(CancellationToken ct);
        public void Complete();
    }
}
