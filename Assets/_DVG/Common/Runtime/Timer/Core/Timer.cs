using System;
using System.Runtime.CompilerServices;

namespace DVG.Common.Timer  
{
    public abstract class Timer : IDisposable {
        public float CurrentTime { get; protected set; }
        public bool IsRunning { get; private set; }

        protected float _initialTime;

        private event Action OnStart, OnTimerStop = delegate { };

        public abstract void Tick();
        public abstract bool IsFinished { get; }

        public virtual void Start() {
            if (IsRunning) return;
            IsRunning = true;

            CurrentTime = _initialTime;
            TimerSystem.RegisterTimer(this);
            OnStart.Invoke();
        }

        public virtual void Stop() {
            if (!IsRunning) return;
            IsRunning = false;

            TimerSystem.DeregisterTimer(this);
            OnTimerStop.Invoke();
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
        public void Reset() => CurrentTime = _initialTime;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetTime(float newTime)
        {
            _initialTime = newTime;            
            CurrentTime = CurrentTime;
        }

        public virtual void Dispose() {
            TimerSystem.DeregisterTimer(this);
            OnStart = null;
            OnTimerStop = null;
        }
    }
}