
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Services
{
    
    public class SceneManagementService 
    {
  
        private static SceneManagementService _instance;

        public static SceneManagementService Instance
        {
            get
            {
                if (_instance == null){
                    _instance = new SceneManagementService();
                }

                return _instance;
            }
        }

        public void ReloadScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void LoadNextScene()
        {
            var currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            var lastSceneIndex = SceneManager.sceneCountInBuildSettings - 1;
    
            if (currentSceneIndex >= lastSceneIndex)
            {
                SceneManager.LoadScene(0);
            }
            else
            {
                SceneManager.LoadScene(currentSceneIndex + 1);
            }
        }

        public void LoadPreviouseScene()
        {
        
            var currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            var lastSceneIndex = SceneManager.sceneCountInBuildSettings - 1;

            if (currentSceneIndex == 0)
            {
                SceneManager.LoadScene(lastSceneIndex);
            }
            else
            {
                SceneManager.LoadScene(currentSceneIndex - 1);
            }
        }
    }
}
