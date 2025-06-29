using UnityEngine;

namespace Assets.Scripts.Audio
{
    public class OptionalAttribute : PropertyAttribute
    {
        public string BoolFieldName { get; }
        public string Label { get; }
        
        public OptionalAttribute(string boolFieldName, string label = null)
        {
            BoolFieldName = boolFieldName;
            Label = label;
        }
    }
}