namespace AudioP
{
    public class ReferencedAudioPlayer : BaseAudioPlayer
    {
        public override void Play()
        {
            AudioManager.Play(soundInfo);
        }
    }
}
