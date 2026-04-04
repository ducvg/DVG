using UnityEngine;
using DVG.Common.Timer;

namespace DVG.UI
{
    [System.Serializable]
    public sealed class InactiveTimeout : ITimeout
    {
        [SerializeField] private float _durationSecs;
        private BaseCanvas _owner;
        private CountdownTimer _timer;

        public InactiveTimeout()
        {
            _timer = new CountdownTimer();
            _timer.OnTimerFinish += OnTimeout;
        }

        public void Run(BaseCanvas owner)
        {
            _owner = owner;
            _timer.SetTime(_durationSecs);
            _timer.Start();
        }

        public void Stop(BaseCanvas owner)
        {
            _timer.Pause();
        }

        private void OnTimeout()
        {
            Object.Destroy(_owner.gameObject);
        }
    }    
}