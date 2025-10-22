using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Core.EntryPoint
{
    public class RuntimeSceneLoader : MonoBehaviour
    {
        [SerializeField] private string defaultNextSceneName;
        private CancellationTokenSource _cancellation;

        async void Start(){

            _cancellation = new CancellationTokenSource();
            var gameInitializer = new GameInitializer();
            await gameInitializer.StartAsync(_cancellation.Token);
            LoadNextScene();
        }

        private void LoadNextScene()
        {
            string sceneToLoad = RuntimeInitializer.SceneToLoadAfterInitialization;

            if (!string.IsNullOrEmpty(sceneToLoad))
            {
                SceneManager.LoadScene(sceneToLoad);
            }
            else
            {
                SceneManager.LoadScene(defaultNextSceneName);
            }
        }
        
        private void OnDestroy()
        {
            _cancellation?.Cancel();
            _cancellation?.Dispose();
        }
    }
}