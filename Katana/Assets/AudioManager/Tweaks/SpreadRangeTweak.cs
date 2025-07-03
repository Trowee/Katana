using System;
using AudioManager.MinMax;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AudioManager.Tweaks
{
    [Serializable]
    public class SpreadRangeTweak : Tweak
    {
        [MinMax(0, 360, "")]
        public Vector2 SpreadRange;
        
        public override AudioSource Apply(AudioSource source)
        {
            source.spread = Random.Range(SpreadRange.x, SpreadRange.y);
            return source;
        }
    }
}