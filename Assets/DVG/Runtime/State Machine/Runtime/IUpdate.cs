namespace DVG.StateMachine
{
    public interface IEarlyUpdate
    {
        public void EarlyUpdate();
    }
    
    public interface IFixedUpdate
    {
        public void FixedUpdate();
    }
    
    public interface IUpdate
    {
        public void Update();
    }
    
    public interface ILateUpdate
    {
        public void LateUpdate();
    }

    public interface IPostLateUpdate
    {
        public void PostLateUpdate();
    }
}