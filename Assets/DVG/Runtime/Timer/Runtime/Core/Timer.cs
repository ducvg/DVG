using System;
using System.Runtime.CompilerServices;

namespace DVG.Timer  
{
    public abstract class Timer : IDisposable {
        public float CurrentTime { get; protected set; }
        public bool IsRunning { get; private set; }
        public event Action OnTimerStart, OnTimerFinish;

        protected float _duration;

        public Timer() { }
        public Timer(float duration)
        {
            _duration = duration;
        }

        public abstract void Tick();
        public abstract bool IsFinished { get; }

        public virtual void Start() {
            if (IsRunning) return;
            IsRunning = true;

            CurrentTime = _duration;
            TimerSystem.RegisterTimer(this);
            OnTimerStart?.Invoke();
        }

        public virtual void Stop() {
            if (!IsRunning) return;
            IsRunning = false;

            TimerSystem.DeregisterTimer(this);
            OnTimerFinish?.Invoke();
        }

        public virtual void Resume()
        {
            if (IsRunning) return;
            IsRunning = true;

            TimerSystem.RegisterTimer(this);
        }

        public virtual void Pause()
        {
            if (!IsRunning) return;
            IsRunning = false;
            TimerSystem.DeregisterTimer(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset() => CurrentTime = _duration;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetDuration(float newDuration)
        {
            _duration = newDuration;            
            CurrentTime = CurrentTime;
        }

        public virtual void Dispose() {
            TimerSystem.DeregisterTimer(this);
            OnTimerStart = null;
            OnTimerFinish = null;
        }
    }
}