namespace DVG.UI
{
    public abstract class Timeout
    {
        protected BaseCanvas _owner;
        protected float _time;

        public Timeout(BaseCanvas owner, float time)
        {
            _owner = owner;
            _time = time;
        }

        public abstract void Run();
        public abstract void Stop();
    }

    public sealed class InactiveTimeout : Timeout
    {
        private float _timer;

        public InactiveTimeout(BaseCanvas owner, float time) : base(owner, time)
        {
        }

        public override void Run()
        {
            _timer = 0;
        }

        public override void Stop()
        {
            _timer = 0;
        }
    }
}