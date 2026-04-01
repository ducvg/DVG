namespace DVG.Audio
{
    public interface IAudioController
    {
        public AudioEmitter Play(IAudioData audioData);
        public bool CanPlay(IAudioData audioData);
        public void Stop(IAudioData audioData);
        public void StopAll();
        // public void SetVolume(float volume); 
    }
}