#if UNITY_EDITOR
using System;

using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

namespace GameplayTags.Editor
{
    public sealed class GameplayTagSelectorWindow : EditorWindow
    {
        private static SerializedProperty _property;

        private GameplayTagsContainer _gameplayTags;

        private readonly Dictionary<string, bool> _foldoutStates = new Dictionary<string, bool>();

        private SerializedObject _serializedObject;

        private bool _showErrorLogs = false;

        private sealed class TagNode
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

        public static void ShowWindow(SerializedProperty property)
        {
            _property = property;
            GameplayTagSelectorWindow window = GetWindow<GameplayTagSelectorWindow>("Select Gameplay Tag");
            window._serializedObject = property.serializedObject;
            window.minSize = new Vector2(300, 400);
            window.Show();
            EditorApplication.update += window.CheckPropertyValidity;
        }

        private void OnEnable()
        {
            _gameplayTags = AssetDatabase.LoadAssetAtPath<GameplayTagsContainer>(GameplayTagsContainer.CONTAINER_PATH);

            if (_gameplayTags == null)
            {
                Debug.LogError($"GameplayTags asset not found at path: {GameplayTagsContainer.CONTAINER_PATH}");
                return;
            }

            BuildTagHierarchy();
        }
        private void OnDisable()
        {
            EditorApplication.update -= CheckPropertyValidity;
        }

        private void CheckPropertyValidity()
        {
            if (_property == null || _property.serializedObject == null || _serializedObject == null)
            {
                Close();
            }
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
            if (index >= parts.Length)
                return;

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
            foreach (var child in node.Children.Values)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(indentLevel * 15f);

                if (child == null)
                {
                    EditorGUILayout.EndHorizontal();

                    continue;
                }

                bool isSelected = false;

                try
                {
                    isSelected = _property.stringValue == child.FullPath;
                }
                catch (Exception ex)
                {
                    if (_showErrorLogs)
                    {
                        DebugHelper.LogWarning($"Closing Tag Editor Window. Cause: {ex.Message}");
                    }

                    EditorGUILayout.EndHorizontal();

                    Close();

                    return;
                }

                if (child.Children.Count > 0)
                {
                    if (!_foldoutStates.ContainsKey(child.FullPath))
                        _foldoutStates[child.FullPath] = false;

                    _foldoutStates[child.FullPath] = EditorGUILayout.Foldout(_foldoutStates[child.FullPath], child.Name, true);
                    EditorGUILayout.EndHorizontal();

                    if (_foldoutStates[child.FullPath])
                    {
                        DrawTagNode(child, indentLevel + 1);
                    }
                }
                else
                {
                    bool newIsSelected = EditorGUILayout.ToggleLeft(child.Name, isSelected);

                    if (newIsSelected && !isSelected)
                    {
                        try
                        {
                            _property.stringValue = child.FullPath;
                            _property.serializedObject.ApplyModifiedProperties();
                            Close();
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError($"Error applying SerializedProperty changes: {ex.Message}");
                        }
                    }

                    EditorGUILayout.EndHorizontal();
                }
            }
        }

    }
}
#endif