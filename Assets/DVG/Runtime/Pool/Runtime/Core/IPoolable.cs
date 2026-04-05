namespace DVG.Pool
{
    public interface IPoolable
    {
        public void OnPoolRent();
        public void OnPoolReturn();
    }
}