using System;
using ArtificeToolkit.Attributes;
using Assets.Scripts.Core;
using Assets.Scripts.Items;
using EasyTextEffects;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.MainMenu.Shop
{
    public class ShopUIScript : MonoBehaviour
    {
        private static ItemManager ItemManager => GameManager.ItemManager;

        [SerializeField, Required, ChildGameObjectsOnly] private Renderer _katana;
        [SerializeField, Required, ChildGameObjectsOnly] private TMP_Text _nameTMP;
        [SerializeField, Required, ChildGameObjectsOnly] private TextEffect _nameEffect;
        [SerializeField, Required, ChildGameObjectsOnly] private TMP_Text _coinsTMP;

        private void Start()
        {
            ItemManager.OnCoinsChanged += UpdateCoins;
            ItemManager.OnItemChanged += UpdateKatana;
            UpdateCoins();
            UpdateKatana();
        }

        private void OnDestroy()
        {
            ItemManager.OnCoinsChanged -= UpdateCoins;
            ItemManager.OnItemChanged -= UpdateKatana;
        }

        private void UpdateCoins() => _coinsTMP.text = $@"₦{ItemManager.Coins}";

        private void UpdateKatana()
        {
            _katana.sharedMaterial = ItemManager.SelectedItem?.Material;
            _nameTMP.text = ItemManager.SelectedItem?.Name;
            ApplyNameEffects();
        }

        private void ApplyNameEffects()
        {
            _nameEffect.globalEffects.Clear();
            if (!ItemManager.SelectedItem) return;
            _nameEffect.globalEffects.AddRange(ItemManager.SelectedItem.NameTextEffects);
            _nameEffect.Refresh();
        }
    }
}
