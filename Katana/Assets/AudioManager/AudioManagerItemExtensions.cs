namespace AudioManager
{
    public static class AudioManagerItemExtensions
    {
        #region Audio Item Override Getters

        private static T GetOverrideValue<T>(
            bool audioOverride, T audioValue,
            bool originalOverride, T originalValue,
            T defaultValue) =>
            audioOverride ? audioValue : originalOverride ? originalValue : defaultValue;

        public static bool PlayOnAwake(this AudioManagerItem item) => GetOverrideValue(
            item.AudioItem.OverridePlayOnAwake, item.AudioItem.PlayOnAwake,
            item.OriginalAudioItem.OverridePlayOnAwake, item.OriginalAudioItem.PlayOnAwake, false);

        public static bool Scaled(this AudioManagerItem item) => GetOverrideValue(
            item.AudioItem.OverrideScaled, item.AudioItem.Scaled,
            item.OriginalAudioItem.OverrideScaled, item.OriginalAudioItem.Scaled, false);

        public static bool FadeIn(this AudioManagerItem item) => GetOverrideValue(
            item.AudioItem.OverrideFadeIn, item.AudioItem.FadeIn,
            item.OriginalAudioItem.OverrideFadeIn, item.OriginalAudioItem.FadeIn, false);

        public static float FadeInTime(this AudioManagerItem item) => GetOverrideValue(
            item.AudioItem.OverrideFadeIn, item.AudioItem.FadeInTime,
            item.OriginalAudioItem.OverrideFadeIn, item.OriginalAudioItem.FadeInTime, 0);

        public static Easings.Type FadeInEasing(this AudioManagerItem item) => GetOverrideValue(
            item.AudioItem.OverrideFadeIn, item.AudioItem.FadeInEasing,
            item.OriginalAudioItem.OverrideFadeIn, item.OriginalAudioItem.FadeInEasing,
            Easings.Type.Linear);

        public static bool FadeInScale(this AudioManagerItem item) => GetOverrideValue(
            item.AudioItem.OverrideFadeIn, item.AudioItem.FadeInScale,
            item.OriginalAudioItem.OverrideFadeIn, item.OriginalAudioItem.FadeInScale,
            true);

        public static bool FadeInScaleWithPitch(this AudioManagerItem item) => GetOverrideValue(
            item.AudioItem.OverrideFadeIn, item.AudioItem.FadeInScaleWithPitch,
            item.OriginalAudioItem.OverrideFadeIn, item.OriginalAudioItem.FadeInScaleWithPitch,
            true);

        public static bool FadeOut(this AudioManagerItem item) => GetOverrideValue(
            item.AudioItem.OverrideFadeOut, item.AudioItem.FadeOut,
            item.OriginalAudioItem.OverrideFadeOut, item.OriginalAudioItem.FadeOut, false);

        public static float FadeOutTime(this AudioManagerItem item) => GetOverrideValue(
            item.AudioItem.OverrideFadeOut, item.AudioItem.FadeOutTime,
            item.OriginalAudioItem.OverrideFadeOut, item.OriginalAudioItem.FadeOutTime, 0);

        public static Easings.Type FadeOutEasing(this AudioManagerItem item) => GetOverrideValue(
            item.AudioItem.OverrideFadeOut, item.AudioItem.FadeOutEasing,
            item.OriginalAudioItem.OverrideFadeOut, item.OriginalAudioItem.FadeOutEasing,
            Easings.Type.Linear);

        public static bool FadeOutScale(this AudioManagerItem item) => GetOverrideValue(
            item.AudioItem.OverrideFadeOut, item.AudioItem.FadeOutScale,
            item.OriginalAudioItem.OverrideFadeOut, item.OriginalAudioItem.FadeOutScale,
            true);

        public static bool FadeOutScaleWithPitch(this AudioManagerItem item) => GetOverrideValue(
            item.AudioItem.OverrideFadeOut, item.AudioItem.FadeOutScaleWithPitch,
            item.OriginalAudioItem.OverrideFadeOut, item.OriginalAudioItem.FadeOutScaleWithPitch,
            true);

        #endregion
    }
}