using System.Collections.Generic;
using System.Linq;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace Assets.Scripts
{

    public class LocalizeDropdown : MonoBehaviour
    {
        [SerializeField] private List<LocalizedString> _dropdownOptions;
        private TMP_Dropdown _tmpDropdown;

        private void Awake()
        {
            if (!_tmpDropdown) _tmpDropdown = GetComponent<TMP_Dropdown>();
        }

        private void Start() => ChangedLocale(LocalizationSettings.SelectedLocale);

        private void OnEnable() => LocalizationSettings.SelectedLocaleChanged += ChangedLocale;

        private void OnDisable() => LocalizationSettings.SelectedLocaleChanged -= ChangedLocale;

        private void ChangedLocale(Locale newLocale)
        {
            var tmpDropdownOptions = _dropdownOptions
                                     .Select(dropdownOption =>
                                                 new TMP_Dropdown.OptionData(
                                                     dropdownOption.GetLocalizedString()))
                                     .ToList();
            _tmpDropdown.options = tmpDropdownOptions;
        }
    }

#if UNITY_EDITOR
    public abstract class AddLocalizeDropdown
    {
        [MenuItem("CONTEXT/TMP_Dropdown/Localize", false, 1)]
        private static void AddLocalizeComponent() =>
            Selection.activeGameObject?.AddComponent<LocalizeDropdown>();
    }
#endif
}