using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace DVG.Timer
{
    public static class TimerRunner 
    {
        private const int _initModifySize = 8;
        private const int _initActiveSize = 64;

        private static readonly HashSet<Timer> _timersToAdd = new(_initModifySize);
        private static readonly HashSet<Timer> _timersToRemove = new(_initModifySize);

        private static readonly HashSet<Timer> _timeScaledTimers = new(_initActiveSize);
        private static readonly HashSet<Timer> _timeUnscaledTimers = new(_initActiveSize);

        public static void UpdateTimers() 
        {            
            foreach(var timer in _timersToAdd)
            {
                switch (timer.DelayType) {
                    case DelayType.Scaled:
                        _timeScaledTimers.Add(timer);
                        break;
                    case DelayType.Unscaled:
                        _timeUnscaledTimers.Add(timer);
                        break;
                }
            } 
            _timersToAdd.Clear();

            foreach(var timer in _timersToRemove)
            {
                switch (timer.DelayType) {
                    case DelayType.Scaled:
                        _timeScaledTimers.Remove(timer);
                        break;
                    case DelayType.Unscaled:
                        _timeUnscaledTimers.Remove(timer);
                        break;
                }
            } 
            _timersToRemove.Clear();

            float deltaTime = Time.deltaTime;
            foreach(var timer in _timeScaledTimers) timer.Tick(deltaTime);

            float unscaledDeltaTime = Time.unscaledDeltaTime;
            foreach(var timer in _timeUnscaledTimers) timer.Tick(unscaledDeltaTime);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RegisterTimer(Timer timer) {
            _timersToRemove.Remove(timer);
            _timersToAdd.Add(timer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DeregisterTimer(Timer timer) {
            _timersToAdd.Remove(timer);
            _timersToRemove.Add(timer);
        }
    }
}