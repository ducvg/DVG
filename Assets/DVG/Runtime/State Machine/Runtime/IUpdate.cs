namespace DVG.StateMachine
{
    public interface IEarlyUpdate
    {
        public void EarlyUpdate();
    }
    
    public interface IUpdate
    {
        public void Update();
    }
    
    public interface ILateUpdate
    {
        public void LateUpdate();
    }
    
    public interface IFixedUpdate
    {
        public void FixedUpdate();
    }
}