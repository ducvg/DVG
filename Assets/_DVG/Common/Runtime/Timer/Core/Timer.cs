using System;

namespace DVG.Timer  {
    public abstract class Timer : IDisposable {
        public float CurrentTime { get; protected set; }
        public bool IsRunning { get; private set; }

        protected float _initialTime;

        private event Action OnStart, OnTimerStop = delegate { };

        protected Timer(float value) {
            _initialTime = value;
        }

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

        public virtual void Reset() => CurrentTime = _initialTime;

        public virtual void Reset(float newDuration)
        {
            _initialTime = newDuration;
            CurrentTime = _initialTime;
        }

        public virtual void Dispose() {
            TimerSystem.DeregisterTimer(this);
            OnStart = null;
            OnTimerStop = null;
        }
    }
}