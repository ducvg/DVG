using System;
using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;
using DVG.UI;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public abstract class BaseCanvas : MonoBehaviour
{
    [SerializeField] protected Canvas _canvas;
    [SerializeField] private TransitionData _transitionData;
    [SerializeField] private float _inactiveTimeout = -1f; //destroy if not used frequently, never if <= 0
    private float _unusedTimer = 0f;
    private bool _isTransitioning = false;

    protected abstract void OnOpen();
    protected abstract void OnClose();
    public abstract bool IsOpen();

    public virtual async UniTask OpenAsync()
    {
        if (_isTransitioning) return;
        _isTransitioning = true;
    
        _unusedTimer = 0f;
        OnOpen();

        await _transitionData.Open(this);
    

        _isTransitioning = false;
    }
    
    public void OpenImmediate()
    {
        if (_isTransitioning) return;
        _isTransitioning = false;

        _unusedTimer = 0f;
        OnOpen();

        _transitionData.CompleteOpen(this);
    }
    
    public virtual async UniTask CloseAsync()
    {
        if (_isTransitioning) return;
        _isTransitioning = true;
    
        await _transitionData.Close(this);
        OnClose();

        _isTransitioning = false;
    }

    public void CloseImmediate()
    {
        if (_isTransitioning) return;
        _isTransitioning = false;

        _transitionData.CompleteClose(this);
        OnClose();
        gameObject.SetActive(false);
    }
}