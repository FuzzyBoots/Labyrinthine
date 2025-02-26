using UnityEditor;
using UnityEngine;
namespace MazeAsset.CustomAttribute.Editor
{
    [CustomPropertyDrawer(typeof(VisibleIfAttribute))]
    public class VisibleIfDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            VisibleIfAttribute visibleIf = (VisibleIfAttribute)attribute;
            string conditionFieldName = visibleIf.conditionFieldName;
            bool compare = visibleIf.compare;

            SerializedProperty conditionField = property.serializedObject.FindProperty(conditionFieldName);

            if (conditionField != null && conditionField.propertyType == SerializedPropertyType.Boolean)
            {
                if (conditionField.boolValue == compare)
                {
                    EditorGUI.PropertyField(position, property, label, true);
                }
            }
            else
            {
                Debug.LogWarning($"VisibleIf: Field '{conditionFieldName}' not found or is not a boolean.");
                EditorGUI.PropertyField(position, property, label, true);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            VisibleIfAttribute visibleIf = (VisibleIfAttribute)attribute;
            SerializedProperty conditionField = property.serializedObject.FindProperty(visibleIf.conditionFieldName);

            if (conditionField != null && conditionField.propertyType == SerializedPropertyType.Boolean && conditionField.boolValue == visibleIf.compare)
            {
                return EditorGUI.GetPropertyHeight(property, label, true);
            }
            return 0;
        }
    }
}