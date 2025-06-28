using System.Collections.Generic;
using System.Linq;
using Scripts;
using UnityEngine;

namespace Assets.Scripts.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public HashSet<AudioClipItem> ClipItems;
        
        public Dictionary<AudioClipItem, AudioSource> ClipItemSources = new();
        public Dictionary<AudioClip, AudioSource> ClipSources = new();

        private void Awake()
        {
            FindAllClipItems();
        }

        private void FindAllClipItems() =>
            ClipItems = Resources.LoadAll<AudioClipItem>("").ToHashSet();

        public void Play(AudioItem audioItem)
        {
            var source = audioItem.SourceType switch
            {
                SourceType.Manager => GetOrAddManagerSource(audioItem),
                _ => null
            };

            source?.Play();
        }

        private AudioSource GetOrAddManagerSource(AudioItem audioItem)
        {
            AudioSource source = null;

            switch (audioItem.ClipAssignmentType)
            {
                case ClipAssignmentType.ClipItem:
                    source = ClipItemSources[audioItem.AudioClipItem];
                    source ??= AddManagerSource(audioItem.AudioClipItem);
                    break;
                case ClipAssignmentType.Clip:
                    source = ClipSources[audioItem.Clip];
                    source ??= AddManagerSource(audioItem.Clip);
                    break;
                case ClipAssignmentType.Name:
                    source = ClipItemSources.FirstOrDefault(x => x.Key.Name == audioItem.ClipName).Value;
                    if (!source)
                        Debug.LogError($"Audio Item named '{audioItem.ClipName}' wasn't be found");
                    break;
            }

            return source;
        }

        private AudioSource AddManagerSource(AudioClipItem item)
        {
            if (!item || !item.Clip) return null;
            
            var source = AddManagerSource(item.Clip);
            ClipItemSources.Add(item, source);
            
            return source;
        }

        private AudioSource AddManagerSource(AudioClip clip)
        {
            if (!clip) return null;

            var source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.clip = clip;
            ClipSources.Add(clip, source);

            return source;
        }
    }
}