using Cysharp.Threading.Tasks;

namespace DVG.UI
{
    public interface ITransition
    {   
        public UniTask Run<T>(T owner) where T : BaseCanvas;
        public void Complete<T>(T owner) where T : BaseCanvas;
    }
}
