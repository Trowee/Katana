using System;
using System.Collections.Generic;
using EasyTextEffects.Editor.MyBoxCopy.Extensions;
using UnityEngine;

namespace Assets.Scripts.Audio
{
    public class AudioManager : MonoBehaviour
    {
        private Transform SourceParent;

        public readonly Dictionary<string, AudioSource> Sources = new();
        private readonly Dictionary<string, GameObject> SourceObjects = new();

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
                Sources.Add(resourceItem.Name, target.AddComponent<AudioSource>());
                SourceObjects.Add(resourceItem.Name, target);
            });

        public AudioSource Play(AudioItem audioItem)
        {
            if (!IsResourceAssignmentTypeValid(audioItem.ResourceAssignmentType)) return null;
            
            var source = audioItem.ApplySettingsToSource(GetOrCreateSource(audioItem));
            source?.Play();
            return source;
        }

        public AudioSource GetOrCreateSource(AudioItem audioItem)
        {
            if (!IsResourceAssignmentTypeValid(audioItem.ResourceAssignmentType)) return null;
            
            GameObject target;
            try
            {
                target = GetOrCreateTarget(audioItem);
            }
            catch (Exception e)
            {
                Debug.LogException(new(
                                       $"(Audio Manager) Failed to get or create target for '{audioItem.Name}'",
                                       e));
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
                                       $"(Audio Manager) Failed to get or create source for '{audioItem.Name}'",
                                       e));
                return null;
            }

            return source;
        }

        private AudioSource GetOrCreateSource(AudioItem audioItem, GameObject target)
        {
            if (Sources.TryGetValue(audioItem.Name, out var source)) return source;

            source = target.AddComponent<AudioSource>();
            if (audioItem.SourceType == SourceType.Manager)
            {
                Sources.Add(audioItem.Name, source);
                SourceObjects.Add(audioItem.Name, target);
            }

            return source;
        }

        private GameObject GetOrCreateTarget(AudioItem audioItem) =>
            audioItem.SourceType switch
            {
                SourceType.Manager =>
                    SourceObjects.TryGetValue(
                        audioItem.Name, out var target)
                        ? target
                        : new(audioItem.Name) { transform = { parent = SourceParent } },
                SourceType.Positional =>
                    new(audioItem.Name)
                    {
                        transform =
                        {
                            parent = SourceParent,
                            position = audioItem.Position
                        }
                    },
                SourceType.Object => audioItem.Target,
                _ => throw new ArgumentOutOfRangeException()
            };

        private bool IsResourceAssignmentTypeValid(ResourceAssignmentType rat)
        {
            if (rat != ResourceAssignmentType.Manual) return true;
            
            Debug.LogError(
                "(Audio Manager) AudioItem ResourceAssignmentType must not be set to 'Manual' at the time of AudioSource creation");
            return false;

        }
    }
}