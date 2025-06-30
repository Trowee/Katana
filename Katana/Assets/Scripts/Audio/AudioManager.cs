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
                        useSettingsPreset: resourceItem.UseItemSettingsPreset,
                        settings: resourceItem.Settings,
                        audioSettingsPreset: resourceItem.SettingsPreset);
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

            var item = GetItem(key, audioItem, out var existingItem)
                           ? existingItem
                           : CreateItem(key, audioItem);

            if (item != null && item.Source != null)
            {
                // OverrideItemSettings check is needed here too
                // It ensures settings aren't reloaded if audioItem doesn't reload every play
                if (audioItem.OverrideItemSettings)
                {
                    if (audioItem.ReloadSettingsEveryPlay)
                        audioItem.ApplySettingsToSource(item.Source);
                }
                else if (item.AudioItem.ReloadSettingsEveryPlay)
                    item.AudioItem.ApplySettingsToSource(item.Source);
            }

            return item;
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
            
            item.OriginalAudioItem = item.AudioItem = audioItem;

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

            audioItem.ApplySettingsToSource(item.Source);
            if (item.Source.playOnAwake) item.Source.Play();
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
            
            return true;
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

            return item.AudioItem.ApplySettingsToSource(item.Source);
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
                SourceType.Object => audioItem.Target,
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