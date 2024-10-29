using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public enum MyScenes
    {
        SW_InitialLoading,
        SW_Managers,
        SW_Menu,
        SW_ArenaScene,
        SW_Tutorial
    }
    public class MySceneManager : MonoBehaviour
    {
        public static MySceneManager instance;

        [HideInInspector] public bool firstLoad = true;

        // Use this for initialization
        void Awake()
        {
            instance = this;
        }
        public void ResetApplication()
        {
            SceneManager.LoadSceneAsync("SW_InitialLoading", LoadSceneMode.Single);
        }

        public AsyncOperation LoadSceneAdditive(string sceneName)
        {
            ShowLoadingPanel();
            var operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            operation.completed += HideLoadingPanel;
            return operation;
        }

        private void HideLoadingPanel(AsyncOperation obj)
        {
            if (GameResourcesManager.instance)
                GameResourcesManager.instance.ShowLoadingPanel(false);
        }
        
        private void ShowLoadingPanel()
        {
            if (GameResourcesManager.instance)
                GameResourcesManager.instance.ShowLoadingPanel(true);
        }

        private string sceneToLoadSave;
        
        /// <summary>
        /// Loads scene only after the second one is unloaded.
        /// </summary>
        /// <param name="sceneName">Scene name for load.</param>
        /// <param name="sceneToUnload">Scene name for unload.</param>
        public void LoadSceneAdditiveAfterUnloading(string sceneName, string sceneToUnload)
        {
            ShowLoadingPanel();
            sceneToLoadSave = sceneName;
            SceneManager.UnloadSceneAsync(sceneToUnload).completed += OnCompleted;
        }

        private void OnCompleted(AsyncOperation obj)
        {
            MySceneManager.instance.LoadSceneAdditive(sceneToLoadSave).completed += HideLoadingPanel;
        }

        public void UnloadScene(string sceneName)
        {
            SceneManager.UnloadSceneAsync(sceneName);
        }
    }
}
