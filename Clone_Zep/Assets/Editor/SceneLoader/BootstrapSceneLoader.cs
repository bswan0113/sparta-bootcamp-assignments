using UnityEditor;
using UnityEditor.SceneManagement;

namespace Editor.SceneLoader
{
    [InitializeOnLoad]
    public static class BootstrapSceneLoader
    {
        private const string PREVIOUS_SCENE_KEY = "zepClone.PreviousScenePath";

        static BootstrapSceneLoader()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                EditorPrefs.SetString(PREVIOUS_SCENE_KEY, EditorSceneManager.GetActiveScene().path);
            }

            if (state == PlayModeStateChange.EnteredEditMode)
            {
                string previousScenePath = EditorPrefs.GetString(PREVIOUS_SCENE_KEY);
                if (!string.IsNullOrEmpty(previousScenePath))
                {
                    EditorSceneManager.OpenScene(previousScenePath);
                    EditorPrefs.DeleteKey(PREVIOUS_SCENE_KEY);
                }
            }
        }
    }
}