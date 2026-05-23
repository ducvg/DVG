using UnityEngine;

namespace DVG.Timers
{
    public class Example : MonoBehaviour
    {
		[SerializeField] private float allDuration = 5f;
		[SerializeField] private bool preserveUpdate = false;
		Timer timerEarlyUpdate, timerUpdate, timerFixedUpdate, timerLateUpdate;


        void Update()
		{
			CheckCreateTimer();
			CheckRunTimer();
			CheckPauseTimer();
			CheckStopTimer();
			if(Input.GetKeyDown(KeyCode.BackQuote))
			{
				timerUpdate.Dispose();
				Debug.Log("Update Timer Disposed");
			}
		}

		private void CheckCreateTimer()
		{
			if(Input.GetKeyDown(KeyCode.Alpha1))
			{
				timerEarlyUpdate = Timer.Create(allDuration, updater: TimerUpdater.EarlyUpdate).BindTo(this)
					.OnStart(() => Debug.Log("EarlyUpdate Timer Started"))
					.OnTick(elapsedTime => Debug.Log($"EarlyUpdate Timer Tick: {elapsedTime}"))
					.OnLoopComplete((completedLoops, elapsedTime) => Debug.Log($"EarlyUpdate Timer Loop Complete: {completedLoops}, Elapsed Time: {elapsedTime}"))
					.OnStop(() => Debug.Log("EarlyUpdate Timer Stopped"));
			}
			if(Input.GetKeyDown(KeyCode.Alpha2))
			{
				timerUpdate = Timer.Create(allDuration, preserve: preserveUpdate).BindTo(this)
					.OnStart(() => Debug.Log("Update Timer Started"))
					.OnTick(elapsedTime => Debug.Log($"Update Timer Tick: {elapsedTime}"))
					.OnLoopComplete((completedLoops, elapsedTime) => Debug.Log($"Update Timer Loop Complete: {completedLoops}, Elapsed Time: {elapsedTime}"))
					.OnStop(() => Debug.Log("Update Timer Stopped"));
			}
			if(Input.GetKeyDown(KeyCode.Alpha3))
			{
				timerFixedUpdate = Timer.Create(allDuration, updater: TimerUpdater.FixedUpdate).BindTo(this)
					.OnStart(() => Debug.Log("FixedUpdate Timer Started"))
					.OnTick(elapsedTime => Debug.Log($"FixedUpdate Timer Tick: {elapsedTime}"))
					.OnLoopComplete((completedLoops, elapsedTime) => Debug.Log($"FixedUpdate Timer Loop Complete: {completedLoops}, Elapsed Time: {elapsedTime}"))
					.OnStop(() => Debug.Log("FixedUpdate Timer Stopped"));
			}
			if(Input.GetKeyDown(KeyCode.Alpha4))
			{
				timerLateUpdate = Timer.Create(allDuration, updater: TimerUpdater.LateUpdate).BindTo(this)
					.OnStart(() => Debug.Log("LateUpdate Timer Started"))
					.OnTick(elapsedTime => Debug.Log($"LateUpdate Timer Tick: {elapsedTime}"))
					.OnLoopComplete((completedLoops, elapsedTime) => Debug.Log($"LateUpdate Timer Loop Complete: {completedLoops}, Elapsed Time: {elapsedTime}"))
					.OnStop(() => Debug.Log("LateUpdate Timer Stopped"));
			}
		}

		private void CheckRunTimer()
		{
			if (Input.GetKeyDown(KeyCode.Q)) timerEarlyUpdate.Run();
			if (Input.GetKeyDown(KeyCode.W)) timerUpdate.Run();
			if (Input.GetKeyDown(KeyCode.E)) timerFixedUpdate.Run();
			if (Input.GetKeyDown(KeyCode.R)) timerLateUpdate.Run();
		}

		private void CheckPauseTimer()
		{
			if (Input.GetKeyDown(KeyCode.A)) timerEarlyUpdate.Pause();
			if (Input.GetKeyDown(KeyCode.S)) timerUpdate.Pause();
			if (Input.GetKeyDown(KeyCode.D)) timerFixedUpdate.Pause();
			if (Input.GetKeyDown(KeyCode.F)) timerLateUpdate.Pause();
		}

		private void CheckStopTimer()
		{
			if (Input.GetKeyDown(KeyCode.Z)) timerEarlyUpdate.Stop();
			if (Input.GetKeyDown(KeyCode.X)) timerUpdate.Stop();	
			if (Input.GetKeyDown(KeyCode.C)) timerFixedUpdate.Stop();
			if (Input.GetKeyDown(KeyCode.V)) timerLateUpdate.Stop();
		}
	}
}