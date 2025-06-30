using System;

namespace Assets.Scripts.Audio.Effects
{
    [Serializable]
    public class AudioEffects
    {
        public Chorus Chorus;
        public Distortion Distortion;
        public Echo Echo;
        public HighPass HighPass;
        public LowPass Lowpass;
        public Reverb Reverb;

        public void ApplyEffects(AudioManagerItem item)
        {
            Chorus.ApplyEffect(item);
            Distortion.ApplyEffect(item);
            Echo.ApplyEffect(item);
            HighPass.ApplyEffect(item);
            Lowpass.ApplyEffect(item);
            Reverb.ApplyEffect(item);
        }

        public void ClearEffects(AudioManagerItem item)
        {
            Chorus.ClearEffect(item);
            Distortion.ClearEffect(item);
            Echo.ClearEffect(item);
            HighPass.ClearEffect(item);
            Lowpass.ClearEffect(item);
            Reverb.ClearEffect(item);
        }
    }
}