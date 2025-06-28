using UnityEngine;

namespace Assets.Scripts.Audio
{
    [CreateAssetMenu(fileName = "ClipItem",
                     menuName = "NnUtils/AudioManager/ClipItem")]
    public class ClipItem : ScriptableObject
    {
        public string Name;
        public AudioClip Clip;
    }
}