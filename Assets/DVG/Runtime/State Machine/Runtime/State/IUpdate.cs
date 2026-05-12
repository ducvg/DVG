namespace DVG.StateMachine
{
    //Must inherit State<TOwner> class 
    public interface IEarlyUpdate
    {
        public void EarlyUpdate(float deltaTime);
    }
    
    public interface IFixedUpdate
    {
        public void FixedUpdate(float fixedDeltaTime);
    }
    
    public interface IUpdate
    {   
        public void Update(float deltaTime);
    }
    
    public interface ILateUpdate
    {
        public void LateUpdate(float deltaTime);
    }

    public interface IPostLateUpdate
    {
        public void PostLateUpdate(float deltaTime);
    }
}