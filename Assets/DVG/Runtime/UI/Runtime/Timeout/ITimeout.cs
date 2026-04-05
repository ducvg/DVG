namespace DVG.UI
{
    public interface ITimeout
    {
        public void Run(BaseCanvas owner);
        public void Stop(BaseCanvas owner);
    }
}