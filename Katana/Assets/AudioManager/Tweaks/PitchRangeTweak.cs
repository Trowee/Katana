using System;
using AudioManager.MinMax;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AudioManager.Tweaks
{
    [Serializable]
    public class PitchRangeTweak :  IAppliable<AudioSource>
    {
        [MinMax(-3, 3, "")]
        public Vector2 PitchRange = Vector2.one;

        public void Apply(AudioSource source) =>
            source.pitch = Random.Range(PitchRange.x, PitchRange.y);
    }
}