#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace GameplayTags.Editor
{
    [CustomEditor(typeof(GameplayTagsContainer))]
    public class GameplayTagsEditor : UnityEditor.Editor
    {
        private SerializedProperty _gameplayTagsList;
        private readonly Dictionary<string, bool> _foldoutStates = new Dictionary<string, bool>();
        private HashSet<string> _existingTags = new HashSet<string>();

        private void OnEnable()
        {
            _gameplayTagsList = serializedObject.FindProperty("_gameplayTagsList");
            CacheExistingTags();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Gameplay Tags", EditorStyles.boldLabel);

            Dictionary<string, List<string>> hierarchy = BuildHierarchy();

            DrawHierarchy(hierarchy, "Root", 0);

            if (GUILayout.Button("Add Root Tag"))
            {
                AddTag("NewTag");
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void CacheExistingTags()
        {
            _existingTags.Clear();
            for (int i = 0; i < _gameplayTagsList.arraySize; i++)
            {
                var element = _gameplayTagsList.GetArrayElementAtIndex(i);
                string tag = element.FindPropertyRelative("<Tag>k__BackingField").stringValue;
                _existingTags.Add(tag);
            }
        }

        private Dictionary<string, List<string>> BuildHierarchy()
        {
            var hierarchy = new Dictionary<string, List<string>>();

            for (int i = 0; i < _gameplayTagsList.arraySize; i++)
            {
                var element = _gameplayTagsList.GetArrayElementAtIndex(i);
                string tag = element.FindPropertyRelative("<Tag>k__BackingField").stringValue;

                string parentTag = GetParentTag(tag);

                if (!hierarchy.ContainsKey(parentTag))
                {
                    hierarchy[parentTag] = new List<string>();
                }

                hierarchy[parentTag].Add(tag);
            }

            return hierarchy;
        }

        private string GetParentTag(string tag)
        {
            int lastDotIndex = tag.LastIndexOf('.');
            return lastDotIndex > 0 ? tag.Substring(0, lastDotIndex) : "Root";
        }

        private void DrawHierarchy(Dictionary<string, List<string>> hierarchy, string parent, int indentLevel)
        {
            if (!hierarchy.ContainsKey(parent))
            {
                return;
            }

            foreach (string tag in hierarchy[parent])
            {
                if (!_foldoutStates.ContainsKey(tag))
                {
                    _foldoutStates[tag] = false;
                }

                EditorGUILayout.BeginHorizontal();

                GUILayout.Space(indentLevel * 15);

                Rect foldoutRect = GUILayoutUtility.GetRect(15, EditorGUIUtility.singleLineHeight, GUILayout.Width(15));
                if (hierarchy.ContainsKey(tag))
                {
                    _foldoutStates[tag] = EditorGUI.Foldout(foldoutRect, _foldoutStates[tag], GUIContent.none, true);
                }

                SerializedProperty tagProperty = FindTagProperty(tag);
                if (tagProperty != null)
                {
                    var tagField = tagProperty.FindPropertyRelative("<Tag>k__BackingField");
                    bool isNativeTag = tagProperty.FindPropertyRelative("<IsNativeTag>k__BackingField").boolValue;

                    GUI.enabled = !isNativeTag;

                    string currentTagValue = tagField.stringValue;
                    string newTagValue = EditorGUILayout.TextField(currentTagValue, GUILayout.ExpandWidth(true));

                    string[] splitTag = newTagValue.Split('.');

                    for (int i = 0; i < splitTag.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(splitTag[i]))
                        {
                            splitTag[i] = "NewTag";
                        }
                    }
          
                    string adjustedTagValue = string.Join(".", splitTag);

                    if (adjustedTagValue != currentTagValue)
                    {
                        if (_existingTags.Contains(adjustedTagValue))
                        {
                            tagField.stringValue = currentTagValue;

                            EditorUtility.DisplayDialog("Tag Exists", $"The tag '{adjustedTagValue}' already exists.", "OK");
                        }
                        else
                        {
                            tagField.stringValue = adjustedTagValue;

                            CacheExistingTags();
                        }
                    }

                    for (int i = 1; i <= splitTag.Length; i++)
                    {
                        string parentTag = string.Join(".", splitTag.Take(i));

                        if (_existingTags.Contains(parentTag))
                        {
                            continue;
                        }

                        _gameplayTagsList.arraySize++;

                        var newElement = _gameplayTagsList.GetArrayElementAtIndex(_gameplayTagsList.arraySize - 1);

                        newElement.FindPropertyRelative("<Tag>k__BackingField").stringValue = parentTag;

                        newElement.FindPropertyRelative("<IsNativeTag>k__BackingField").boolValue = false;

                        CacheExistingTags();
                    }

                    GUI.enabled = true;
                }
                  
                if (GUILayout.Button("+", GUILayout.Width(30)))
                {
                    AddTag(tag + ".NewTag");
                }

                if (tagProperty != null && !tagProperty.FindPropertyRelative("<IsNativeTag>k__BackingField").boolValue)
                {
                    if (GUILayout.Button("-", GUILayout.Width(30)))
                    {
                        RemoveTagAndChildren(tag);
                    }
                }

                EditorGUILayout.EndHorizontal();

                if (_foldoutStates[tag])
                {
                    DrawHierarchy(hierarchy, tag, indentLevel + 1);
                }
            }
        }

        private SerializedProperty FindTagProperty(string tagName)
        {
            for (int i = 0; i < _gameplayTagsList.arraySize; i++)
            {
                var element = _gameplayTagsList.GetArrayElementAtIndex(i);
                string tag = element.FindPropertyRelative("<Tag>k__BackingField").stringValue;
                if (tag == tagName)
                {
                    return element;
                }
            }
            return null;
        }

        private void AddTag(string parentTag)
        {
            string[] tagParts = parentTag.Split('.');
            string accumulatedTag = string.Empty;

            for (int i = 0; i < tagParts.Length; i++)
            {
                accumulatedTag = string.IsNullOrEmpty(accumulatedTag) ? tagParts[i] : accumulatedTag + "." + tagParts[i];

                if (_existingTags.Contains(accumulatedTag))
                {
                    continue;
                }

                _gameplayTagsList.arraySize++;

                var newElement = _gameplayTagsList.GetArrayElementAtIndex(_gameplayTagsList.arraySize - 1);

                newElement.FindPropertyRelative("<Tag>k__BackingField").stringValue = accumulatedTag;

                newElement.FindPropertyRelative("<IsNativeTag>k__BackingField").boolValue = false;

                CacheExistingTags();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void RemoveTagAndChildren(string parentTag)
        {
            if (!EditorUtility.DisplayDialog(
                "Confirm Delete",
                $"Are you sure you want to delete the tag '{parentTag}' and all its child tags?",
                "Yes",
                "No"))
            {
                return;
            }

            List<string> tagsToRemove = new List<string>();

            FindChildTags(parentTag, tagsToRemove);

            for (int i = _gameplayTagsList.arraySize - 1; i >= 0; i--)
            {
                SerializedProperty element = _gameplayTagsList.GetArrayElementAtIndex(i);

                string tag = element.FindPropertyRelative("<Tag>k__BackingField").stringValue;

                if (tagsToRemove.Contains(tag))
                {
                    _gameplayTagsList.DeleteArrayElementAtIndex(i);
                }
            }

            CacheExistingTags();
            serializedObject.ApplyModifiedProperties();
        }

        private void FindChildTags(string parentTag, List<string> tagsToRemove)
        {
            tagsToRemove.Add(parentTag);

            for (int i = 0; i < _gameplayTagsList.arraySize; i++)
            {
                var element = _gameplayTagsList.GetArrayElementAtIndex(i);
                string tag = element.FindPropertyRelative("<Tag>k__BackingField").stringValue;

                if (GetParentTag(tag) == parentTag)
                {
                    FindChildTags(tag, tagsToRemove);
                }
            }
        }
    }
}
#endif
