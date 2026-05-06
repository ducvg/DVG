namespace DVG.StateMachine
{
    public enum UpdateType
    {
        EarlyUpdate = 1,
        FixedUpdate = 2,
        Update = 3,
        LateUpdate = 4, //PreLateUpdate
        PostLateUpdate = 5
    }
}