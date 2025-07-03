using System;
using AudioManager.MinMax;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AudioManager.Tweaks
{
    [Serializable]
    public class DopplerLevelRangeTweak : ITweak<AudioSource>
    {
        [MinMax(0, 5, "")]
        public Vector2 DopplerLevelRange = Vector2.one;
        
        public void Apply(AudioSource source) => source.dopplerLevel = Random.Range(DopplerLevelRange.x, DopplerLevelRange.y);
    }
}