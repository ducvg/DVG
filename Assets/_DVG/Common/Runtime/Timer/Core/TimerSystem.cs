using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DVG.Timer {
    public static class TimerSystem {
        private static HashSet<Timer> s_activeTimers = new(64);
        private static HashSet<Timer> s_timersToAdd = new(8);
        private static HashSet<Timer> s_timersToRemove = new(8);

        public static void UpdateTimers() {
            foreach(var timer in s_timersToAdd) s_activeTimers.Add(timer);
            s_timersToAdd.Clear();

            foreach(var timer in s_timersToRemove) s_activeTimers.Remove(timer);
            s_timersToRemove.Clear();

            foreach(var timer in s_activeTimers) timer.Tick();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RegisterTimer(Timer timer) => s_activeTimers.Add(timer);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DeregisterTimer(Timer timer) => s_activeTimers.Remove(timer);
    }
}