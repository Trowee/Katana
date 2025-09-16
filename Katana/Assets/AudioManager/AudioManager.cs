using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if STEAMAUDIO_ENABLED

using SteamAudio;

#endif

namespace AudioManager
{
    public class AudioManager : MonoBehaviour
    {
        public readonly Dictionary<AudioManagerKey, HashSet<AudioManagerItem>> Items = new();
        private Transform _sourceParent;

#if STEAMAUDIO_ENABLED

        private SteamAudioProbeBatch _steamAudioProbeBatch;
        public SteamAudioProbeBatch SteamAudioProbeBatch =>
            _steamAudioProbeBatch ??= FindFirstObjectByType<SteamAudioProbeBatch>();

#endif

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
            var foundItem = TryGetItem(key, audioItem, out var existingItem);
            var item = foundItem ? existingItem : CreateItem(key, audioItem);

            item.ApplySettings(!foundItem).ApplyEffects().ApplySteamAudioSourceSettings();
            if (!foundItem && item.PlayOnAwake()) item.Play();
            return item;
        }

        private bool TryGetItem(AudioManagerKey key, AudioItem audioItem,
                                out AudioManagerItem item)
        {
            item = null;
            if (!audioItem.ReuseSource) return false;

            if (!Items.TryGetValue(key, out var items)) return false;

            foreach (var i in items.Where(i => i.AudioItem.Name == audioItem.Name &&
                                               i.AudioItem.AsChildObject == audioItem.AsChildObject))
            {
                if (audioItem.SourceType == SourceType.Object)
                {
                    var t = audioItem.AsChildObject ? i.transform.parent.gameObject : i.gameObject;
                    if (t != audioItem.Target) continue;
                }

                // This should only get overriden if using an AudioResourceItem
                // It allows for predictable tweaks stacking between AudioItem and OriginalAudioItem
                // Also allows for tweaks reload each play
                if (audioItem.ResourceAssignmentType == ResourceAssignmentType.ResourceItem)
                    i.OriginalAudioItem = new(audioItem.AudioResourceItem);
                i.AudioItem = audioItem;
                item = i;
                return true;
            }

            return false;
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
                    $"(Audio Manager) Failed to create Target Object for '{audioItem.Name}'", e));
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
                    $"(Audio Manager) Failed to create Audio Source for '{audioItem.Name}'", e));
                return null;
            }

#if STEAMAUDIO_ENABLED

            try
            {
                item.SteamAudioSource = CreateSteamAudioSource(item);
            }
            catch (Exception e)
            {
                Debug.LogException(new(
                    $"(Audio Manager) Failed to get or create Steam Audio Source for '{audioItem.Name}'", e));
                return null;
            }

#endif

            return item;
        }

        public void RemoveItem(AudioManagerItem item)
        {
            if (Items.TryGetValue(item.Key, out var items)) items.Remove(item);
        }

        private AudioSource CreateSource(AudioManagerKey key, AudioManagerItem item)
        {
            var Source = item.gameObject.AddComponent<AudioSource>();

            if (item.AudioItem.ReuseSource)
            {
                if (!Items.TryGetValue(key, out var items))
                {
                    items = new();
                    Items.Add(key, items);
                }

                items.Add(item);
            }

            return Source;
        }

#if STEAMAUDIO_ENABLED

        private SteamAudioSource CreateSteamAudioSource(AudioManagerItem item) =>
            (item.AudioItem.OverrideSteamAudioSource
            ? item.AudioItem.UseSteamAudioSource
            : item.OriginalAudioItem.UseSteamAudioSource)
                ? item.gameObject.AddComponent<SteamAudioSource>()
                : null;

#endif

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