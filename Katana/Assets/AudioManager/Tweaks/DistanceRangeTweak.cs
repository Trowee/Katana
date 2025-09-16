using System;
using AudioManager.MinMax;
using UnityEngine;

namespace AudioManager.Tweaks
{
    [Serializable]
    public class DistanceRangeTweak : IAppliable<AudioSource>
    {
        [MinMax(0, 10000, "")]
        public Vector2 DistanceRange = new(1, 500);

        public void Apply(AudioSource source)
        {
            source.minDistance = DistanceRange.x;
            source.maxDistance = DistanceRange.y;
        }
    }
}