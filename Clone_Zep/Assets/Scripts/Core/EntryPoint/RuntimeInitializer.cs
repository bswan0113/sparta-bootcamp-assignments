// 네임스페이스: Core.EntryPoint
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.EntryPoint
{
    public static class RuntimeInitializer
    {
        internal static string SceneToLoadAfterInitialization { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void InitializeBeforeSceneLoad()
        {
            #if UNITY_EDITOR
            string bootstrapScenePath = "Assets/Scenes/Initialization.unity";
            const string PREVIOUS_SCENE_KEY = "zepClone.PreviousScenePath";

            if (SceneManager.GetActiveScene().path != bootstrapScenePath)
            {
                var scenePathToLoad = EditorPrefs.GetString(PREVIOUS_SCENE_KEY);
                EditorPrefs.DeleteKey(PREVIOUS_SCENE_KEY);
                if (!string.IsNullOrEmpty(scenePathToLoad))
                {
                    SceneToLoadAfterInitialization = scenePathToLoad;
                }
                SceneManager.LoadScene(bootstrapScenePath);
            }
            #endif
        }
    }
}