using System;
using System.Collections.Generic;
using EasyTextEffects.Editor.MyBoxCopy.Extensions;
using UnityEngine;

namespace Assets.Scripts.Audio
{
    public class AudioManager : MonoBehaviour
    {
        private Transform SourceParent;
        
        private readonly Dictionary<AudioItem, GameObject> _audioSourceObjects = new();
        public readonly Dictionary<AudioResourceItem, AudioSource> ResourceItemSources = new();

        private void Awake()
        {
            SourceParent = new GameObject("AudioSources").transform;
            SourceParent.SetParent(transform);
            
            LoadClipItems();
        }

        private void LoadClipItems() =>
            Resources.LoadAll<AudioResourceItem>("").ForEach(resourceItem =>
            {
                GameObject target = new(resourceItem.Name);
                target.transform.SetParent(SourceParent);
                ResourceItemSources.Add(resourceItem, target.AddComponent<AudioSource>());
            });

        public AudioSource Play(AudioItem audioItem)
        {
            var source = GetOrCreateSource(audioItem);
            source?.Play();
            return source;
        }

        public AudioSource GetOrCreateSource(AudioItem audioItem)
        {
            GameObject target;
            try
            {
                target = GetOrCreateTarget(audioItem);
            }
            catch (Exception e)
            {
                Debug.LogException(new(
                    $"(Audio Manager) Failed to get or create target for '{audioItem.Name}'", e));
                return null;
            }

            AudioSource source;
            try
            {
                source = GetOrCreateSource(audioItem, target);
            }
            catch (Exception e)
            {
                Debug.LogException(new(
                    $"(Audio Manager) Failed to get or create source for '{audioItem.Name}'", e));
                return null;
            }

            return source;
        }

        private AudioSource GetOrCreateSource(AudioItem audioItem, GameObject target)
        {
            if (ResourceItemSources.TryGetValue(audioItem.ResourceItem, out var source))
                return source;
            
            source = target.AddComponent<AudioSource>();
            if (audioItem.SourceType == SourceType.Manager)
                ResourceItemSources.Add(audioItem.ResourceItem, source);

            return source;
        }

        private GameObject GetOrCreateTarget(AudioItem audioItem) =>
            audioItem.SourceType switch
            {
                SourceType.Manager => (new GameObject(audioItem.Name)
                                       .transform.parent = SourceParent)
                                      .GetChild(SourceParent.childCount).gameObject,
                SourceType.Positional => new($"{audioItem.Name}_AudioSource"),
                SourceType.Attached => audioItem.AttachTarget,
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}