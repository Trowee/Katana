using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AudioManager
{
    public class AudioManager : MonoBehaviour
    {
        public readonly Dictionary<AudioManagerKey, HashSet<AudioManagerItem>> Items = new();
        private Transform _sourceParent;

        private void Awake()
        {
            _sourceParent = new GameObject("AudioSources").transform;
            _sourceParent.SetParent(transform);

            LoadClipItems();
        }

        private void LoadClipItems()
        {
            foreach (var resourceItem in Resources.LoadAll<AudioResourceItem>(""))
                GetOrCreateItem(new(resourceItem));
        }

        public AudioManagerItem Play(AudioItem audioItem)
        {
            if (!AreAudioItemSettingsValid(audioItem)) return null;
            var item = GetOrCreateItem(audioItem);
            item?.Play();
            return item;
        }

        public AudioManagerItem GetOrCreateItem(AudioItem audioItem)
        {
            if (!AreAudioItemSettingsValid(audioItem)) return null;

            AudioManagerKey key = new(audioItem.SourceType, audioItem.Name);
            var foundItem = GetItem(key, audioItem, out var existingItem);
            var item = foundItem ? existingItem : CreateItem(key, audioItem);

            item.ApplySettings(!foundItem).ApplyEffects();
            if (!foundItem && item.PlayOnAwake()) item.Play();
            return item;
        }

        private bool GetItem(AudioManagerKey key, AudioItem audioItem,
                             out AudioManagerItem item)
        {
            item = null;
            if (!audioItem.ReuseSource) return false;

            if (!Items.TryGetValue(key, out var items)) return false;

            item = items.FirstOrDefault(x => x.AudioItem.Name == key.Name);
            if (item == null) return false;

            if (audioItem.SourceType == SourceType.Object &&
                item.gameObject != audioItem.Target)
            {
                item = null;
                return false;
            }
            
            // This should only get overriden if using an AudioResourceItem
            // It allows for predictable settings stacking between AudioItem and OriginalAudioItem
            // Also allows for settings reload each play
            item.OriginalAudioItem =
                audioItem.ResourceAssignmentType == ResourceAssignmentType.ResourceItem
                    ? new(audioItem.AudioResourceItem)
                    : item.OriginalAudioItem;
            item.AudioItem = audioItem;
            
            return true;
        }

        private AudioManagerItem CreateItem(AudioManagerKey key, AudioItem audioItem)
        {
            AudioManagerItem item;

            try
            {
                item = CreateSourceObject(key, audioItem).AddComponent<AudioManagerItem>();
            }
            catch (Exception e)
            {
                Debug.LogException(new(
                                       $"(Audio Manager) Failed to create target for '{audioItem.Name}'",
                                       e));
                return null;
            }

            item.Manager = this;
            item.Key = key;
            item.OriginalAudioItem =
                audioItem.ResourceAssignmentType == ResourceAssignmentType.ResourceItem
                    ? new(audioItem.AudioResourceItem)
                    : audioItem;
            item.AudioItem = audioItem;

            try
            {
                item.Source = CreateSource(key, item);
            }
            catch (Exception e)
            {
                Debug.LogException(new(
                                       $"(Audio Manager) Failed to create source for '{audioItem.Name}'",
                                       e));
                return null;
            }

            return item;
        }

        public void RemoveItem(AudioManagerItem item)
        {
            if (Items.TryGetValue(item.Key, out var items)) items.Remove(item);
        }

        private AudioSource CreateSource(AudioManagerKey key, AudioManagerItem item)
        {
            item.Source = item.gameObject.AddComponent<AudioSource>();

            if (item.AudioItem.ReuseSource)
            {
                if (!Items.TryGetValue(key, out var items))
                {
                    items = new();
                    Items.Add(key, items);
                }

                items.Add(item);
            }

            return item.Source;
        }

        private GameObject CreateSourceObject(AudioManagerKey key, AudioItem audioItem) =>
            key.SourceType switch
            {
                SourceType.Manager => new(key.Name) { transform = { parent = _sourceParent } },
                SourceType.Positional =>
                    new(key.Name)
                    {
                        transform =
                        {
                            parent = _sourceParent,
                            position = audioItem.Position
                        }
                    },
                SourceType.Object => audioItem.AsChildObject
                                         ? new(key.Name)
                                             { transform = { parent = audioItem.Target.transform } }
                                         : audioItem.Target,
                _ => throw new ArgumentOutOfRangeException()
            };

        private bool AreAudioItemSettingsValid(AudioItem audioItem)
        {
            var rat = audioItem.ResourceAssignmentType;

            if (rat == ResourceAssignmentType.Manual)
            {
                Debug.LogError(
                    $"(Audio Manager) AudioItem ResourceAssignmentType must not be set to '{rat}' at the time of AudioSource creation");
                return false;
            }

            if (rat == ResourceAssignmentType.Name &&
                audioItem.SourceType != SourceType.Manager)
            {
                Debug.LogError(
                    $"(Audio Manager) AudioItem ResourceAssignmentType must not be set to '{rat}' when SourceType is not set to {SourceType.Manager}");
                return false;
            }

            return true;
        }
    }
}