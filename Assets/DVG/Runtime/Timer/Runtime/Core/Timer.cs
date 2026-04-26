using System;
using System.Runtime.CompilerServices;

namespace DVG.Timer  
{
    public delegate void TimerDelegate();
    public enum DelayType
    {
        Scaled,
        Unscaled
    }

    public abstract class Timer : IDisposable {
        public float CurrentTime { get; protected set; }
        public bool IsRunning { get; protected set; }
        public DelayType DelayType { get; protected set; }

        private event TimerDelegate _onResume, _onPause;
        private event TimerDelegate _onStart, onFinish;

        protected float _duration;

        public abstract void Tick(float deltaDelayTime);
        public abstract bool IsFinished { get; }

        public virtual void Start() {
            if (IsRunning) return;
            IsRunning = true;

            CurrentTime = _duration;
            TimerRunner.RegisterTimer(this);
            _onStart?.Invoke();
        }

        public virtual void Finish() {
            if (!IsRunning) return;
            IsRunning = false;

            TimerRunner.DeregisterTimer(this);
            onFinish?.Invoke();
        }

        public virtual void Resume()
        {
            if (IsRunning) return;
            IsRunning = true;

            TimerRunner.RegisterTimer(this);
            _onResume?.Invoke();
        }

        public virtual void Pause()
        {
            if (!IsRunning) return;
            IsRunning = false;
            
            TimerRunner.DeregisterTimer(this);
            _onPause?.Invoke();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnStart(TimerDelegate callback) => _onStart = callback;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnResume(TimerDelegate callback) => _onResume = callback;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnPause(TimerDelegate callback) => _onPause = callback;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnFinish(TimerDelegate callback) => onFinish = callback;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset() => CurrentTime = _duration;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetDuration(float newDuration)
        {
            _duration = newDuration;            
            CurrentTime = CurrentTime;
        }

        public virtual void Dispose() {
            TimerRunner.DeregisterTimer(this);
            _onStart = null;
            onFinish = null;
        }
    }
}