using Cysharp.Threading.Tasks;
using DVG.UI;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public abstract class BaseCanvas : MonoBehaviour
{
    [SerializeField] private TransitionData _transitionData;
    [SerializeField] private ITimeout _timeout;
    private bool _isTransitioning = false;

    protected abstract void OnOpen();
    protected abstract void OnClose();
    public abstract bool IsOpen();

    public virtual async UniTask OpenAsync()
    {
        if (_isTransitioning) return;
        _isTransitioning = true;
    
        _timeout?.Stop();
        OnOpen();

        await _transitionData.Open(this);
    

        _isTransitioning = false;
    }
    
    public void OpenImmediate()
    {
        if (_isTransitioning) return;
        _isTransitioning = false;

        _timeout?.Stop();
        OnOpen();

        _transitionData.CompleteOpen(this);
    }
    
    public virtual async UniTask CloseAsync()
    {
        if (_isTransitioning) return;
        _isTransitioning = true;
    
        await _transitionData.Close(this);

        _timeout?.Run();
        OnClose();

        _isTransitioning = false;
    }

    public void CloseImmediate()
    {
        if (_isTransitioning) return;
        _isTransitioning = false;

        _transitionData.CompleteClose(this);

        _timeout?.Run();
        OnClose();
    }
}