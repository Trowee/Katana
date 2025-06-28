using UnityEngine;

namespace Assets.Scripts.Audio
{
    [CreateAssetMenu(fileName = "AudioClipItem",
                     menuName = "NnUtils/Audio Manager/Audio Clip Item")]
    public class AudioClipItem : ScriptableObject
    {
        public string Name;
        public AudioClip Clip;
    }
}