using UnityEngine;

namespace GameplayTags
{
    public static class GameplayTagsExtensionMethods
    {
        public static bool HasGameplayTag(this Component component, string tag)
        {
            return component.TryGetComponent(out GameplayTagsComponent gameplayTagsComponent) && gameplayTagsComponent.HasTag(tag);
        }

        public static bool HasGameplayTag(this Behaviour behaviour, string tag)
        {
            return behaviour.TryGetComponent(out GameplayTagsComponent gameplayTagsComponent) && gameplayTagsComponent.HasTag(tag);
        }

        public static bool HasGameplayTag(this GameObject gameObject, string tag)
        {
            return gameObject.TryGetComponent(out GameplayTagsComponent gameplayTagsComponent) && gameplayTagsComponent.HasTag(tag);
        }

        public static bool TryAddGameplayTag(this Component component, string tag)
        {
            return TryAddGameObjectTag(component.gameObject, tag);
        }

        public static bool TryAddGameplayTag(this Behaviour behaviour, string tag)
        {
            return TryAddGameObjectTag(behaviour.gameObject, tag);
        }

        public static bool TryAddGameplayTag(this GameObject gameObject, string tag)
        {
            return TryAddGameObjectTag(gameObject, tag);
        }

        public static bool TryRemoveGameplayTag(this Component component, string tag)
        {
            return TryRemoveGameObjectTag(component.gameObject, tag);
        }

        public static bool TryRemoveGameplayTag(this Behaviour behaviour, string tag)
        {
            return TryRemoveGameObjectTag(behaviour.gameObject, tag);
        }

        public static bool TryRemoveGameplayTag(this GameObject gameObject, string tag)
        {
            return TryRemoveGameObjectTag(gameObject, tag);
        }

        private static bool TryAddGameObjectTag(GameObject gameObject, string tag)
        {
            if (gameObject.TryGetComponent(out GameplayTagsComponent gameplayTagsComponent))
            {
                gameplayTagsComponent.AddTag(tag);

                return true;
            }

            return false;
        }

        private static bool TryRemoveGameObjectTag(GameObject gameObject, string tag)
        {
            if (gameObject.TryGetComponent(out GameplayTagsComponent gameplayTagsComponent))
            {
                gameplayTagsComponent.RemoveTag(tag);

                return true;
            }

            return false;
        }
    }
}
