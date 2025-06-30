using System;
using System.Collections.Generic;
using System.Linq;
using EasyTextEffects.Editor.MyBoxCopy.Extensions;
using UnityEngine;

namespace Assets.Scripts.Audio
{
    public class AudioManager : MonoBehaviour
    {
        private Transform _sourceParent;

        public readonly Dictionary<AudioManagerKey, HashSet<AudioManagerItem>> Items = new();

        private void Awake()
        {
            _sourceParent = new GameObject("AudioSources").transform;
            _sourceParent.SetParent(transform);

            LoadClipItems();
        }

        private void LoadClipItems() =>
            Resources.LoadAll<AudioResourceItem>("").ForEach(resourceItem =>
            {
                AudioItem audioItem =
                    new(ResourceAssignmentType.ResourceItem,
                        audioResourceItem: resourceItem,
                        overrideMixerGroup: true,
                        mixerGroup: resourceItem.MixerGroup,
                        sourceType: SourceType.Manager,
                        reuseSource: true,
                        overridePlayOnAwake: true,
                        playOnAwake: resourceItem.PlayOnAwake,
                        position: resourceItem.Position,
                        overrideItemSettings: true,
                        reloadSettingsEveryPlay: resourceItem.ReloadSettingsEveryPlay,
                        useItemSettingsPreset: resourceItem.UseItemSettingsPreset,
                        itemSettings: resourceItem.ItemSettings,
                        itemSettingsPreset: resourceItem.ItemSettingsPreset);
                GetOrCreateItem(audioItem);
            });

        public AudioManagerItem Play(AudioItem audioItem)
        {
            if (!AreAudioItemSettingsValid(audioItem)) return null;
            
            var item = GetOrCreateItem(audioItem);
                
            item?.Source?.Play();
            return item;
        }

        public AudioManagerItem GetOrCreateItem(AudioItem audioItem)
        {
            if (!AreAudioItemSettingsValid(audioItem)) return null;

            AudioManagerKey key = new(audioItem.SourceType, audioItem.Name);
            AudioManagerItem item = new(audioItem);
            
            item = GetItem(key, item, out var existingItem)
                       ? existingItem
                       : CreateItem(key, item);

            if (item != null && item.Source != null)
            {
                // OverrideItemSettings check is needed here too
                // It ensures settings aren't reloaded if audioItem doesn't reload every play
                if (audioItem.OverrideItemSettings && audioItem.ReloadSettingsEveryPlay)
                    audioItem.ApplySettingsToSource(item.Source);
                else if (item.AudioItem.ReloadSettingsEveryPlay)
                    item.AudioItem.ApplySettingsToSource(item.Source);
            }

            return item;
        }

        private AudioManagerItem CreateItem(AudioManagerKey key, AudioManagerItem item)
        {
            try
            {
                item.Object = CreateSourceObject(key, item);
            }
            catch (Exception e)
            {
                Debug.LogException(new(
                                       $"(Audio Manager) Failed to create target for '{item.AudioItem.Name}'",
                                       e));
                return null;
            }

            try
            {
                CreateSource(key, item);
            }
            catch (Exception e)
            {
                Debug.LogException(new(
                                       $"(Audio Manager) Failed to create source for '{item.AudioItem.Name}'",
                                       e));
                return null;
            }

            item.AudioItem.ApplySettingsToSource(item.Source);
            return item;
        }

        private bool GetItem(AudioManagerKey key, AudioManagerItem item,
                             out AudioManagerItem existingItem)
        {
            existingItem = null;
            if (!item.AudioItem.ReuseSource) return false;

            if (!Items.TryGetValue(key, out var items)) return false;
            
            existingItem = items.FirstOrDefault(x => x.AudioItem.Name == key.Name);
            if (existingItem == null) return false;

            if (item.AudioItem.SourceType == SourceType.Object &&
                existingItem.Object != item.Object)
            {
                existingItem = null;
                return false;
            }
            
            return true;
        }

        private AudioSource CreateSource(AudioManagerKey key, AudioManagerItem item)
        {
            item.Source = item.Object.AddComponent<AudioSource>();

            if (item.AudioItem.ReuseSource)
            {
                if (!Items.TryGetValue(key, out var items))
                {
                    items = new();
                    Items.Add(key, items);
                }

                items.Add(item);
            }

            return item.AudioItem.ApplySettingsToSource(item.Source);
        }

        private GameObject CreateSourceObject(AudioManagerKey key, AudioManagerItem item) =>
            key.SourceType switch
            {
                SourceType.Manager => new(key.Name) { transform = { parent = _sourceParent } },
                SourceType.Positional =>
                    new(key.Name)
                    {
                        transform =
                        {
                            parent = _sourceParent,
                            position = item.AudioItem.Position
                        }
                    },
                SourceType.Object => item.AudioItem.Target,
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