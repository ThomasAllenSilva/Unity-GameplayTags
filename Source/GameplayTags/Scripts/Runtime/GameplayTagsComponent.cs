using System;

using System.Collections.Generic;

using UnityEngine;

namespace GameplayTags
{
    public sealed class GameplayTagsComponent : MonoBehaviour
    {
        public event Action<string> AddedTag;

        public event Action<string> RemovedTag;

        public IReadOnlyDictionary<string, GameplayTag> Tags => _gameplayTags;

        private readonly Dictionary<string, GameplayTag> _gameplayTags = new Dictionary<string, GameplayTag>();

        [SerializeField] private List<string> _startupTags = new List<string>();

        private void Awake()
        {
            InitializeTags();

#if UNITY_EDITOR == false
            _startupTags = null;
#endif
        }

        private void InitializeTags()
        {
            _gameplayTags.Clear();

            _gameplayTags.TrimExcess();

            foreach (string tag in _startupTags)
            {
                if (tag == string.Empty || tag == null)
                {
                    continue;
                }

                _gameplayTags.TryAdd(tag, GameplayTagsContainer.GetGameplayTagByName(tag));
            }
        }

        public bool HasTag(string tag)
        {
            return _gameplayTags.ContainsKey(tag);
        }

        public void AddTag(string tag)
        {
            _gameplayTags.TryAdd(tag, GameplayTagsContainer.GetGameplayTagByName(tag));

            AddedTag?.Invoke(tag);
        }

        public void RemoveTag(string tag)
        {
            _gameplayTags.Remove(tag);

            RemovedTag?.Invoke(tag);
        }

        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                InitializeTags();
            }
        }
    }
}
