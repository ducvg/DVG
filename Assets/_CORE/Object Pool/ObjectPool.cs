namespace DVG.Pool
{
    public sealed class ObjectPool<T> : IObjectPool<T>
    {
        

        public void Preload(int amount)
        {
            throw new System.NotImplementedException();
        }

        public T Rent()
        {
            throw new System.NotImplementedException();
        }

        public void Return(T instance)
        {
            throw new System.NotImplementedException();
        }
        public void Clear()
        {
            throw new System.NotImplementedException();
        }
    }
}