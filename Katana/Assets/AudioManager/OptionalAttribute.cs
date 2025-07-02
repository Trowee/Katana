using UnityEngine;

namespace AudioManager
{
    public class OptionalAttribute : PropertyAttribute
    {
        public OptionalAttribute(string boolFieldName, string label = null,
                                 bool displayCheckbox = true)
        {
            BoolFieldName = boolFieldName;
            Label = label;
            DisplayCheckbox = displayCheckbox;
        }

        public string BoolFieldName { get; }
        public string Label { get; }
        public bool DisplayCheckbox { get; }
    }
}