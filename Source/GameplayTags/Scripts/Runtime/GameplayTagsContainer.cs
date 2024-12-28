using System.Collections.Generic;

using UnityEngine;

namespace GameplayTags
{
    [CreateAssetMenu(fileName = "GameplayTagsContainer", menuName = "GameplayTags/GameplayTagsContainer")]
    public sealed class GameplayTagsContainer : ScriptableObject
    {
        public List<GameplayTag> GameplayTagsList => _gameplayTagsList;

        private static readonly Dictionary<string, GameplayTag> _tags = new Dictionary<string, GameplayTag>();

        [SerializeField] private List<GameplayTag> _gameplayTagsList = new List<GameplayTag>();

        private void Awake()
        {
            RefreshTags();
        }

        private void OnEnable()
        {
            RefreshTags();
        }

        private void RefreshTags()
        {
            _tags.Clear();

            _tags.TrimExcess();

            foreach (GameplayTag gameplayTag in _gameplayTagsList)
            {
                _tags.TryAdd(gameplayTag.Tag, gameplayTag);
            }
        }

        public static GameplayTag GetGameplayTagByName(string tagName)
        {
            DebugHelper.CheckCondition(_tags.ContainsKey(tagName) == false, $"GameplayTag: '{tagName}' does not exist. Make sure to create it before using it");

            return _tags[tagName];
        }

#if UNITY_EDITOR
      
        public void ResetNativeTags()
        {
            foreach (GameplayTag gameplayTag in _gameplayTagsList.ToArray())
            {
                if (gameplayTag.IsNativeTag)
                {
                    _gameplayTagsList.Remove(gameplayTag);
                }
            }
        }

        [ContextMenu("Force Native Tags Reset")]
        public void ForceNativeTagsReset()
        {
            foreach (GameplayTag gameplayTag in _gameplayTagsList.ToArray())
            {
                if (gameplayTag.IsNativeTag)
                {
                    _gameplayTagsList.Remove(gameplayTag);
                }
            }

            NativeGameplayTags.InitializeNativeTags();
        }

        public void AddNativeTag(string tag)
        {
            EnsureParentTagsExist(tag);

            UpdateNativeTagsHierarchy(tag);

            if (TagExists(tag) == false)
            {
                _gameplayTagsList.Add(new GameplayTag(tag, true));
            }
        }

        private void UpdateNativeTagsHierarchy(string nativeTag)
        {
            string[] tagParts = nativeTag.Split('.');

            string currentTag = string.Empty;

            for (int i = 0; i < tagParts.Length; i++)
            {
                currentTag = string.IsNullOrEmpty(currentTag) ? tagParts[i] : $"{currentTag}.{tagParts[i]}";

                for (int j = _gameplayTagsList.Count - 1; j >= 0; j--)
                {
                    var gameplayTag = _gameplayTagsList[j];

                    if (gameplayTag.Tag == currentTag && !gameplayTag.IsNativeTag)
                    {
                        _gameplayTagsList[j] = new GameplayTag(currentTag, true);
                    }
                }
            }
        }

        private void EnsureParentTagsExist(string tag)
        {
            string parentTag = GetParentTag(tag);

            if (!string.IsNullOrEmpty(parentTag) && !TagExists(parentTag))
            {
                AddNativeTag(parentTag);
            }
        }

        private string GetParentTag(string tag)
        {
            int lastDotIndex = tag.LastIndexOf('.');

            return lastDotIndex > 0 ? tag.Substring(0, lastDotIndex) : null;
        }

        private bool TagExists(string tag)
        {
            foreach (var entry in _gameplayTagsList)
            {
                if (entry.Tag == tag)
                {
                    return true;
                }
            }
            return false;
        }

        private void Reset()
        {
            NativeGameplayTags.InitializeNativeTags();
        }
#endif
    }
}
