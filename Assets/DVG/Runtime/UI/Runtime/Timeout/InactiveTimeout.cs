using UnityEngine;
using DVG.Timers;

namespace DVG.UI
{
    [System.Serializable]
    public sealed class InactiveTimeout : Timeout
    {
		[SerializeField] private float timeoutDuration = 120f;
        private Timer _timer;
		
		public override void Setup(BaseCanvas owner)
		{
			_timer = Timer.Create(timeoutDuration).BindTo(owner)
						.OnComplete(owner, OnTimeout);
		}

		public override void OnOpen()
		{
			_timer.Pause();
			_timer.ResetTime();
		}

		public override void OnClose()
		{
			_timer.Run();
		}

		private static void OnTimeout(BaseCanvas owner)
		{
			Object.Destroy(owner);
		}
    }    
}