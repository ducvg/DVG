namespace DVG.UI
{
    public abstract class ITimeout
    {
        protected BaseCanvas _owner;
        protected float _time;


        public abstract void Run();
        public abstract void Stop();
    }
}