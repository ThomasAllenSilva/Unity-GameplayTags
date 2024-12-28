#if UNITY_EDITOR
using UnityEditor;

using UnityEngine;

namespace GameplayTags.Editor
{
    [CustomEditor(typeof(GameplayTagsComponent))]
    public sealed class GameplayTagsComponentEditor : UnityEditor.Editor
    {
        private SerializedProperty _tagsProperty;

        private void OnEnable()
        {
            _tagsProperty = serializedObject.FindProperty("_startupTags");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Startup Tags", EditorStyles.boldLabel);

            EditorGUILayout.Space();

            for (int i = 0; i < _tagsProperty.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();

                SerializedProperty tagProperty = _tagsProperty.GetArrayElementAtIndex(i);

                DrawTagSelector(tagProperty.stringValue, i);

                if (GUILayout.Button("Remove", GUILayout.Width(70)))
                {
                    _tagsProperty.DeleteArrayElementAtIndex(i);
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Add Tag"))
            {
                _tagsProperty.InsertArrayElementAtIndex(_tagsProperty.arraySize);

                _tagsProperty.GetArrayElementAtIndex(_tagsProperty.arraySize - 1).stringValue = "";
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawTagSelector(string currentTag, int index)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginDisabledGroup(true);

            EditorGUILayout.TextField(currentTag);

            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button("...", GUILayout.Width(30)))
            {
                GameplayTagListSelectorWindow.ShowWindow(_tagsProperty, index);
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}
#endif