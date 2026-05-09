namespace DVG.StateMachine
{
    public interface IEarlyUpdate : IStateStatus
    {
        public void EarlyUpdate();
    }
    
    public interface IFixedUpdate : IStateStatus
    {
        public void FixedUpdate();
    }
    
    public interface IUpdate : IStateStatus
    {
        public void Update();
    }
    
    public interface ILateUpdate : IStateStatus
    {
        public void LateUpdate();
    }

    public interface IPostLateUpdate : IStateStatus
    {
        public void PostLateUpdate();
    }
}