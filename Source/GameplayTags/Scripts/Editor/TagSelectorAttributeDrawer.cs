#if UNITY_EDITOR
using UnityEditor;

using UnityEngine;

namespace GameplayTags.Editor
{
    [CustomPropertyDrawer(typeof(GameplayTagSelectorAttribute))]
    public sealed class GameplayTagSelectorDrawer : PropertyDrawer
    {
        private const float BUTTON_WIDTH = 60f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            EditorGUI.BeginDisabledGroup(true);

            Rect fieldRect = new Rect(position.x, position.y, position.width - BUTTON_WIDTH, position.height);

            property.stringValue = EditorGUI.TextField(fieldRect, label, property.stringValue);

            EditorGUI.EndDisabledGroup();

            Rect buttonRect = new Rect(position.x + position.width - BUTTON_WIDTH, position.y, BUTTON_WIDTH, position.height);

            if (GUI.Button(buttonRect, "Edit Tag"))
            {
                GameplayTagSelectorWindow.ShowWindow(property);
            }

            EditorGUI.EndProperty();
        }
    }
}
#endif