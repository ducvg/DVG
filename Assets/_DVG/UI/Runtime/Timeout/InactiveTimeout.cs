using UnityEngine;

namespace DVG.UI
{
    [System.Serializable]
    public sealed class InactiveTimeout : ITimeout
    {
        [SerializeField] private float _duration;
        private float _timer;

        public void Run()
        {
            _timer = 0;
        }

        public void Stop()
        {
            _timer = 0;
        }
    }    
}