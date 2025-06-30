using UnityEngine;

namespace Assets.Scripts.Audio
{
    public class OptionalAttribute : PropertyAttribute
    {
        public string BoolFieldName { get; }
        public string Label { get; }
        public bool DisplayCheckbox { get; }

        public OptionalAttribute(string boolFieldName, string label = null,
                                 bool displayCheckbox = true)
        {
            BoolFieldName = boolFieldName;
            Label = label;
            DisplayCheckbox = displayCheckbox;
        }
    }
}