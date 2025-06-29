using UnityEngine;

namespace Assets.Scripts.Audio
{
    public class AudioManagerItem
    {
        public AudioItem AudioItem;
        public AudioSource Source;
        public GameObject Object;

        public AudioManagerItem(AudioItem audioItem = null,
                                AudioSource source = null,
                                GameObject obj = null)
        {
            AudioItem = audioItem;
            Source = source;
            Object = !obj && audioItem?.SourceType == SourceType.Object ? audioItem.Target : obj;
        }
    }
}