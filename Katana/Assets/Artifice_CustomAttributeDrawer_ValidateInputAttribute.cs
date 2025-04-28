using System;
using System.Reflection;
using ArtificeToolkit.Attributes;
using ArtificeToolkit.Editor.Resources;
using UnityEditor;
using UnityEngine;

namespace ArtificeToolkit.Editor.Artifice_CustomAttributeDrawers.CustomAttributeDrawer_Validators
{
    [Artifice_CustomAttributeDrawer(typeof(ValidateInputAttribute))]
    public class
        Artifice_CustomAttributeDrawer_ValidateInputAttribute :
        Artifice_CustomAttributeDrawer_Validator_BASE
    {
        private string _logMessage = "";
        public override string LogMessage => _logMessage;
        public override Sprite LogSprite { get; } = Artifice_SCR_CommonResourcesHolder.instance.ErrorIcon;
        public override LogType LogType { get; } = LogType.Error;

        protected override bool IsApplicableToProperty(SerializedProperty property) => true;

        public override bool IsValid(SerializedProperty property)
        {
            // Return true if this is an element of an Array
            if (property.IsArrayElement()) return true;
            
            object targetObject = property.serializedObject.targetObject;
            var targetType = targetObject.GetType();
            var fieldName = property.propertyPath.Split('.')[0];
            var fieldInfo = targetType.GetField(fieldName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (fieldInfo == null)
            {
                _logMessage = $"Artifice ValidateInput: Invalid property name: {property.name}";
                return false;
            }

            var validateAttribute = fieldInfo.GetCustomAttribute<ValidateInputAttribute>();
            var condition = validateAttribute.Condition;
            _logMessage = validateAttribute.Message;
            
            var validationCondition = targetType.GetMethod(condition,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            
            if (validationCondition == null)
            {
                _logMessage = $"Artifice ValidateInput: Invalid validation condition: {condition}";
                return false;
            }
            
            if (validationCondition.ReturnType != typeof(bool))
            {
                _logMessage = $"Artifice ValidateInput: Invalid return type for the condition method: {condition}";
                return false;
            }
            
            try
            {
                var result = validationCondition.Invoke(targetObject, null);
                if (result is bool isValid) return isValid;
            }
            catch (TargetInvocationException ex)
            {
                _logMessage = $"Artifice ValidateInput: Exception occurred while invoking validation method '{condition}' for property '{property.name}' on '{targetType.Name}'.\nInner Exception: {ex.InnerException?.Message ?? ex.Message}";
                return false;
            }
            catch (Exception ex)
            {
                _logMessage = $"Artifice ValidateInput: Exception occured while processing validation for property '{property.name}' on '{targetType.Name}'.\nException: {ex.Message}";
                 return false;
            }

            return false;
        }
    }
}