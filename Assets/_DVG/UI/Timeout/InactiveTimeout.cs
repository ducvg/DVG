namespace DVG.UI
{
    public sealed class InactiveTimeout : ITimeout
    {
        private float _timer;

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