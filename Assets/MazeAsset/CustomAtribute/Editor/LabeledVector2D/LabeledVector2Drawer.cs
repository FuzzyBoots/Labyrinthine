using UnityEditor;
using UnityEngine;
namespace MazeAsset.CustomAttribute.Editor
{
    [CustomPropertyDrawer(typeof(LabeledVector2Attribute))]
    public class LabeledVector2Drawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            LabeledVector2Attribute splitVector2Attribute = (LabeledVector2Attribute)attribute;
            if (property.propertyType != SerializedPropertyType.Vector2)
            {
                EditorGUI.LabelField(position, label.text, "Use LabeledVector2Attribute with Vector2.");
                return;
            }

            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;

            float fieldWidth = (position.width - spacing) / 2;
            Rect labelXRect = new Rect(position.x, position.y, fieldWidth, lineHeight);
            Rect labelYRect = new Rect(position.x + fieldWidth + spacing, position.y, fieldWidth, lineHeight);

            Rect fieldXRect = new Rect(position.x, position.y + lineHeight + spacing, fieldWidth, lineHeight);
            Rect fieldYRect = new Rect(position.x + fieldWidth + spacing, position.y + lineHeight + spacing, fieldWidth, lineHeight);

            EditorGUI.LabelField(labelXRect, splitVector2Attribute.XLabel);
            EditorGUI.LabelField(labelYRect, splitVector2Attribute.YLabel);

            property.vector2Value = new Vector2(
                EditorGUI.FloatField(fieldXRect, property.vector2Value.x),
                EditorGUI.FloatField(fieldYRect, property.vector2Value.y)
            );
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;
            return (lineHeight + spacing) * 2;
        }
    }
}