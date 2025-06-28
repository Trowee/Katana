using System;
using System.Collections.Generic;
using System.Linq;
using EasyTextEffects.Editor.MyBoxCopy.Extensions;
using UnityEngine;

namespace Assets.Scripts.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public Dictionary<AudioClipItem, AudioSource> ClipItemSources = new();
        public Dictionary<AudioClip, AudioSource> ClipSources = new();

        private void Awake()
        {
            LoadClipItems();
        }

        private void LoadClipItems() =>
            Resources.LoadAll<AudioClipItem>("").ForEach(x => AddSource(x, gameObject));

        public AudioSource Play(AudioItem audioItem)
        {
            var source = GetOrCreateSource(audioItem);
            source.Play();
            return source;
        }

        public AudioSource GetOrCreateSource(AudioItem audioItem)
        {
            if (audioItem.ClipAssignmentType == ClipAssignmentType.Manual)
            {
                Debug.LogError(
                    "Audio Manager: Audio Item Clip Assignment Type must not be set to 'Manual' at the time of calling the Play function");
                return null;
            }

            GameObject target;
            try
            {
                target = GetOrCreateTarget(audioItem);
            }
            catch (Exception e)
            {
                Debug.LogError(
                    $"Audio Manager: Failed to get or create target for '{audioItem.Name}'\n{e}");
                return null;
            }

            AudioSource source;
            try
            {
                source = GetOrCreateSource(audioItem, target);
            }
            catch (Exception e)
            {
                Debug.LogError(
                    $"Audio Manager: Failed to get or create source for '{audioItem.Name}'\n{e}");
                return null;
            }

            return source;
        }

        private AudioSource GetOrCreateSource(AudioItem audioItem, GameObject target)
        {
            AudioSource source;

            switch (audioItem.ClipAssignmentType)
            {
                case ClipAssignmentType.ClipItem:
                    source = ClipItemSources[audioItem.AudioClipItem];
                    source ??= AddSource(audioItem.AudioClipItem, target);
                    if (source && audioItem.SourceType == SourceType.Manager)
                        ClipItemSources.Add(audioItem.AudioClipItem, source);
                    break;
                case ClipAssignmentType.Clip:
                    source = ClipSources[audioItem.Clip];
                    source ??= AddSource(audioItem.Clip, target);
                    if (source && audioItem.SourceType == SourceType.Manager)
                        ClipSources.Add(audioItem.Clip, source);
                    break;
                case ClipAssignmentType.Name:
                    source = ClipItemSources.FirstOrDefault(x => x.Key.Name == audioItem.ClipName).Value;
                    if (!source)
                        throw new KeyNotFoundException(
                            $"Audio Item named '{audioItem.ClipName}' wasn't be found");
                    break;
                case ClipAssignmentType.Manual:
                    throw new InvalidOperationException(
                        "Audio Item Clip Assignment Type must not be set to 'Manual' at the time of calling the Play function");
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return source;
        }

        private GameObject GetOrCreateTarget(AudioItem audioItem) =>
            audioItem.SourceType switch
            {
                SourceType.Manager => gameObject,
                SourceType.Positional => new($"{audioItem.Name}_AudioSource"),
                SourceType.Attached => audioItem.AttachTarget,
                _ => throw new ArgumentOutOfRangeException()
            };

        private static AudioSource AddSource(AudioClipItem item, GameObject target)
        {
            if (!item) throw new NullReferenceException("Audio Clip Item can't be null");
            return AddSource(item.Clip, target);
        }

        private static AudioSource AddSource(AudioClip clip, GameObject target)
        {
            if (!clip) throw new NullReferenceException("Audio Clip can't be null");

            var source = target.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.clip = clip;

            return source;
        }
    }
}