#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameplayTags
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public sealed class NativeGameplayTags
    {
        public const string NONE = "None";

        //public const string TEST_GAMEPLAY_TAG = "Test.NewTag";

#if UNITY_EDITOR
        private const string CONTAINER_PATH = "Assets/Plugins/Thomas/GameplayTags/Data/GameplayTagsContainer.asset";

        private static GameplayTagsContainer _gameplayTags;

        private static void EnsureGameplayTags()
        {
            if (_gameplayTags == null)
            {
                _gameplayTags = AssetDatabase.LoadAssetAtPath<GameplayTagsContainer>(CONTAINER_PATH);
            }
        }

        static NativeGameplayTags()
        {
            InitializeNativeTags();
        }

        public static void InitializeNativeTags()
        {
            EnsureGameplayTags();

            if(_gameplayTags == null)
            {
                UnityEngine.Debug.LogWarning("Could Not Initialize Native Tags. Make sure the path and name is correct");
            }

            _gameplayTags.ResetNativeTags();

            //_gameplayTags.AddNativeTag(TEST_GAMEPLAY_TAG);
        }
#endif
    }
}
