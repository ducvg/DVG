using UnityEngine;
using DVG.Timer;

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
            _timer = new CountdownTimer(_durationSecs);
            _timer.OnTimerStop += OnTimeout;
        }

        public void Run(BaseCanvas owner)
        {
            _owner = owner;
            _timer.SetDuration(_durationSecs);
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