using System;
using AudioManager.MinMax;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AudioManager.Tweaks
{
    [Serializable]
    public class DistanceRangeTweak : ITweak<AudioSource>
    {
        [MinMax(0, 10000, "")]
        public Vector2 DistanceRange = new(1, 500);

        public void Apply(AudioSource source) =>
            source.dopplerLevel = Random.Range(DistanceRange.x, DistanceRange.y);
    }
}