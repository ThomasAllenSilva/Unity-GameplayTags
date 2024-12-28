using System;

using UnityEngine;

namespace GameplayTags
{
    [Serializable]
    public sealed class GameplayTag
    {
        [field: SerializeField, HideInInspector] public bool IsNativeTag { get; private set; }

        [field: GameplayTagSelector]
        [field: SerializeField] public string Tag { get; private set; }

        public GameplayTag()
        {

        }

        public GameplayTag(string tag, bool isNativeTag = false)
        {
            Tag = tag;

            IsNativeTag = isNativeTag;
        }
    }
}
