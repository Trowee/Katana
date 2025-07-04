using System;
using AudioManager.MinMax;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AudioManager.Tweaks
{
    [Serializable]
    public class SpreadRangeTweak : IAppliable<AudioSource>
    {
        [MinMax(0, 360, "")]
        public Vector2 SpreadRange;
        
        public void Apply(AudioSource source) => source.spread = Random.Range(SpreadRange.x, SpreadRange.y);
    }
}