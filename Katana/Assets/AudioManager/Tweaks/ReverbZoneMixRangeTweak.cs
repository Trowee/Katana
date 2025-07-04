using System;
using AudioManager.MinMax;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AudioManager.Tweaks
{
    [Serializable]
    public class ReverbZoneMixRangeTweak : IAppliable<AudioSource>
    {
        [MinMax(0, 1.1f, "")]
        public Vector2 ReverbZoneMixRange = Vector2.one;

        public void Apply(AudioSource source) =>
            source.reverbZoneMix = Random.Range(ReverbZoneMixRange.x, ReverbZoneMixRange.y);
    }
}