namespace DVG.StateMachine
{
    public interface IEarlyUpdate : IState
    {
        public void EarlyUpdate(float deltaTime);
    }
    
    public interface IFixedUpdate : IState
    {
        public void FixedUpdate(float fixedDeltaTime);
    }
    
    public interface IUpdate : IState
    {   
        public void Update(float deltaTime);
    }
    
    public interface ILateUpdate : IState
    {
        public void LateUpdate(float deltaTime);
    }

    public interface IPostLateUpdate : IState
    {
        public void PostLateUpdate(float deltaTime);
    }
}