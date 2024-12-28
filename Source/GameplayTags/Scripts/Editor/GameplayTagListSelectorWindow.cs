#if UNITY_EDITOR
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

namespace GameplayTags.Editor
{
    public sealed class GameplayTagListSelectorWindow : EditorWindow
    {
        private static SerializedProperty _tagsProperty;
        private static int _tagIndex;
        private GameplayTagsContainer _gameplayTags;
        private readonly Dictionary<string, bool> _foldoutStates = new Dictionary<string, bool>();

        private bool _throwExceptions = false;

        private class TagNode
        {
            public string FullPath { get; }
            public string Name { get; }
            public Dictionary<string, TagNode> Children { get; } = new Dictionary<string, TagNode>();

            public TagNode(string fullPath, string name)
            {
                FullPath = fullPath;
                Name = name;
            }
        }

        private TagNode _rootNode;

        public static void ShowWindow(SerializedProperty tagsProperty, int tagIndex)
        {
            _tagsProperty = tagsProperty;
            _tagIndex = tagIndex;

            var window = GetWindow<GameplayTagListSelectorWindow>("Select Gameplay Tag");
            window.minSize = new Vector2(300, 400);
            window.maxSize = window.minSize;
            window.Show();
        }

        private void OnEnable()
        {
            _gameplayTags = AssetDatabase.LoadAssetAtPath<GameplayTagsContainer>("Assets/Plugins/Thomas/GameplayTags/Data/GameplayTags.asset");

            if (_gameplayTags == null)
            {
                Debug.LogError("GameplayTags asset not found at path: Assets/Plugins/GameplayTags/Data/GameplayTags.asset");
                return;
            }

            BuildTagHierarchy();
        }

        private void OnGUI()
        {
            if (_gameplayTags == null)
            {
                EditorGUILayout.LabelField("GameplayTags asset is missing!");
                return;
            }

            EditorGUILayout.LabelField("Select a Gameplay Tag", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            DrawTagNode(_rootNode, 0);
        }

        private void BuildTagHierarchy()
        {
            _rootNode = new TagNode("", "");

            foreach (var tag in _gameplayTags.GameplayTagsList)
            {
                string[] parts = tag.Tag.Split('.');
                AddTagToHierarchy(_rootNode, parts, 0, "");
            }
        }

        private void AddTagToHierarchy(TagNode currentNode, string[] parts, int index, string currentPath)
        {
            if (index >= parts.Length) return;

            string partName = parts[index];
            string fullPath = string.IsNullOrEmpty(currentPath) ? partName : $"{currentPath}.{partName}";

            if (!currentNode.Children.ContainsKey(partName))
            {
                currentNode.Children[partName] = new TagNode(fullPath, partName);
            }

            AddTagToHierarchy(currentNode.Children[partName], parts, index + 1, fullPath);
        }

        private void DrawTagNode(TagNode node, int indentLevel)
        {
            try
            {
                foreach (var child in node.Children.Values)
                {
                    EditorGUILayout.BeginHorizontal();

                    GUILayout.Space(indentLevel * 15f);

                    if (child.Children.Count > 0)
                    {
                        if (!_foldoutStates.ContainsKey(child.FullPath))
                            _foldoutStates[child.FullPath] = false;

                        _foldoutStates[child.FullPath] = EditorGUILayout.Foldout(_foldoutStates[child.FullPath], "", true, EditorStyles.foldout);

                        GUILayout.Space(-5);

                        bool isSelected = _tagsProperty.GetArrayElementAtIndex(_tagIndex).stringValue == child.FullPath;
                        bool newIsSelected = EditorGUILayout.Toggle(isSelected, GUILayout.Width(15));

                        EditorGUILayout.LabelField(child.Name, GUILayout.Width(500));
                        if (newIsSelected && !isSelected)
                        {
                            _tagsProperty.GetArrayElementAtIndex(_tagIndex).stringValue = child.FullPath;
                            _tagsProperty.serializedObject.ApplyModifiedProperties();
                            Close();
                        }

                        EditorGUILayout.EndHorizontal();

                        if (_foldoutStates[child.FullPath])
                        {
                            DrawTagNode(child, indentLevel + 1);
                        }
                    }
                    else
                    {
                        GUILayout.Space(49);

                        bool isSelected = _tagsProperty.GetArrayElementAtIndex(_tagIndex).stringValue == child.FullPath;
                        bool newIsSelected = EditorGUILayout.Toggle(isSelected, GUILayout.Width(15));
                        EditorGUILayout.LabelField(child.Name, GUILayout.ExpandWidth(true));

                        if (newIsSelected && !isSelected)
                        {
                            _tagsProperty.GetArrayElementAtIndex(_tagIndex).stringValue = child.FullPath;
                            _tagsProperty.serializedObject.ApplyModifiedProperties();
                            Close();
                        }

                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
            catch (System.Exception)
            {
                if (_throwExceptions)
                {
                    throw;
                }

                Close();

                EditorGUILayout.EndHorizontal();
            }
        }
    }
}
#endif