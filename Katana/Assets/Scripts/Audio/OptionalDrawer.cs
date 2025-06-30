#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Audio
{
    [CustomPropertyDrawer(typeof(OptionalAttribute))]
    public class OptionalAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var optionalAttr = (OptionalAttribute)attribute;
            var labelText = optionalAttr.Label;
            label.text = labelText ?? label.text;
            SerializedProperty boolProp = null;

            var propertyPath = property.propertyPath;
            var lastDotIndex = propertyPath.LastIndexOf('.');
            if (lastDotIndex >= 0)
            {
                var parentPath = propertyPath[..lastDotIndex];
                boolProp =
                    property.serializedObject.FindProperty(
                        $"{parentPath}.{optionalAttr.BoolFieldName}");
            }

            // If not found as sibling, try at root level
            boolProp ??= property.serializedObject.FindProperty(optionalAttr.BoolFieldName);

            if (boolProp == null)
            {
                EditorGUI.LabelField(position, label.text,
                                     $"Bool field '{optionalAttr.BoolFieldName}' not found");
                return;
            }

            EditorGUI.BeginProperty(position, label, property);

            var toggleRect = new Rect(position.x, position.y, 20, position.height);
            var fieldRect = new Rect(position.x + (optionalAttr.DisplayCheckbox ? 25 : 0),
                                     position.y, position.width - 25, position.height);

            if (optionalAttr.DisplayCheckbox)
                boolProp.boolValue = EditorGUI.Toggle(toggleRect, boolProp.boolValue);

            using (new EditorGUI.DisabledScope(!boolProp.boolValue))
            {
                EditorGUI.PropertyField(fieldRect, property, label, true);
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}
#endif