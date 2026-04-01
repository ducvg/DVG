using System;
using PrimeTween;

namespace DVG.Timer  {
    public abstract class Timer : IDisposable {
        public float CurrentTime { get; protected set; }
        public bool IsRunning { get; private set; }

        protected float initialTime;

        private event Action OnStart, OnTimerStop  = delegate { };

        protected Timer(float value) {
            initialTime = value;
        }

        public abstract void Tick();
        public abstract bool IsFinished { get; }

        public void Start() {
            if (IsRunning) return;
            IsRunning = true;

            CurrentTime = initialTime;
            TimerSystem.RegisterTimer(this);
            OnStart.Invoke();
        }

        public void Stop() {
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

        public virtual void Reset() => CurrentTime = initialTime;

        public virtual void Dispose() {
            TimerSystem.DeregisterTimer(this);
        }
    }
}