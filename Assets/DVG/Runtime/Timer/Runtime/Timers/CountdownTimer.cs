using UnityEngine;

namespace DVG.Timer
{
    public sealed class CountdownTimer : Timer
    {
        public override void Tick(float deltaDelayTime)
        {
            if (IsRunning && CurrentTime > 0) 
            {
                CurrentTime -= deltaDelayTime;
            }

            if (IsRunning && CurrentTime <= 0) 
            {
                Finish();
            }
        }

        public override bool IsFinished => CurrentTime <= 0;
    }
}
