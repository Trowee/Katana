using ArtificeToolkit.Attributes;
using Assets.Scripts.Core;
using Assets.Scripts.Items;
using EasyTextEffects;
using NnUtils.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;

namespace Assets.Scripts.MainMenu.Shop
{
    [ExecuteAlways]
    public class ShopUIScript : MonoBehaviour
    {
        private static ItemManager ItemManager => GameManager.ItemManager;

        [SerializeField, Required, ChildGameObjectsOnly] private Renderer _katana;

        [SerializeField, Required, ChildGameObjectsOnly]
        [OnValueChanged(nameof(HandleNameTMPChanged))]
        private TMP_Text _nameTMP;
        [SerializeField, Required, ChildGameObjectsOnly] private TextEffect _nameEffect;

        [SerializeField, HideInInspector] private LocalizeStringEvent _nameLocalizeStringEvent;

        [SerializeField, Required, ChildGameObjectsOnly] private TMP_Text _coinsTMP;

        private void OnEnable()
        {
            HandleNameTMPChanged();
        }

        private void Start()
        {
            if (!Application.isPlaying) return;

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

        private void HandleNameTMPChanged()
        {
            if (_nameLocalizeStringEvent)
                _nameLocalizeStringEvent.OnUpdateString.RemoveListener(UpdateNameText);
            _nameLocalizeStringEvent =
                _nameTMP?.gameObject.GetOrAddComponent<LocalizeStringEvent>();
            if (_nameLocalizeStringEvent)
                _nameLocalizeStringEvent.OnUpdateString.AddListener(UpdateNameText);
        }

        private void UpdateNameText(string localizedText)
        {
            _nameTMP.text = localizedText;
            ApplyNameEffects();
        }

        private void UpdateCoins() => _coinsTMP.text = $@"₦{ItemManager.Coins}";

        private void UpdateKatana()
        {
            _katana.sharedMaterial = ItemManager.SelectedItem?.Material;
            var ls = ItemManager.SelectedItem?.LocalizedString;
            _nameLocalizeStringEvent.StringReference
                                    .SetReference(ls.TableReference, ls.TableEntryReference);
            ls.RefreshString();
            _nameLocalizeStringEvent.RefreshString();
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
