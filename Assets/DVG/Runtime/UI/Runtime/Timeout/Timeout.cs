namespace DVG.UI
{
    public abstract class Timeout
    {
		public abstract void Setup(BaseCanvas owner);
        public abstract void OnOpen();
        public abstract void OnClose();
    }
}