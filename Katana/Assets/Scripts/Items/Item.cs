using System;
using System.Collections.Generic;
using ArtificeToolkit.Attributes;
using UnityEngine;
using EasyTextEffects;
using UnityEngine.Localization;

namespace Assets.Scripts.Items
{
    [CreateAssetMenu(fileName = "Item", menuName = "Item")]
    public class Item : ScriptableObject
    {
        [OnValueChanged(nameof(HandlePropertyChanged))]
        public string Name;
        [OnValueChanged(nameof(HandlePropertyChanged))]
        public int Price;
        [OnValueChanged(nameof(HandlePropertyChanged))]
        public bool UnlockedByDefault;
        [OnValueChanged(nameof(HandlePropertyChanged))]
        public Material Material;
        [OnValueChanged(nameof(HandlePropertyChanged))]
        public List<GlobalTextEffectEntry> NameTextEffects;
        [OnValueChanged(nameof(HandlePropertyChanged))]
        public LocalizedString LocalizedString;

        public event Action OnPropertyChanged;

        private void HandlePropertyChanged() => OnPropertyChanged?.Invoke();

        public event Action OnUnlocked;

        public bool Unlocked
        {
            get => PlayerPrefs.GetInt($"{Name}Unlocked", 0) == 1 || UnlockedByDefault;
            set
            {
                PlayerPrefs.SetInt($"{Name}Unlocked", value ? 1 : 0);
                OnUnlocked?.Invoke();
            }
        }
    }
}