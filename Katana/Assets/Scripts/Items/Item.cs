using System;
using UnityEngine;

namespace Assets.Scripts.Items
{
    [CreateAssetMenu(fileName = "Item", menuName = "Item")]
    public class Item : ScriptableObject
    {
        public string Name;
        public float Price;
        public bool UnlockedByDefault;
        public Material Material;

        public bool Unlocked
        {
            get => PlayerPrefs.GetInt("Unlocked", 0) == 1 || UnlockedByDefault;
            set
            {
                PlayerPrefs.SetInt("Unlocked", value ? 1 : 0);
                OnUnlocked?.Invoke();
            }
        }

        public event Action OnUnlocked;
    }
}