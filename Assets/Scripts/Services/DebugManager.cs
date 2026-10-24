
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

using Assets.Scripts.Player;

namespace Assets.Scripts.Services
{
    public class DebugManager : MonoBehaviour
    {
    
        //Imports
        private static DebugManager instance; 
        private SceneManagementService _sceneManagementService;
        private Transform _playerRocket; 
        private PlayerCollision _playerCollision; 

        #region Init

        void Awake()
        {

            _sceneManagementService = SceneManagementService.Instance;

            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
        
            instance = this;

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            
            _playerRocket = player.transform; 
            _playerCollision = _playerRocket.GetComponent<PlayerCollision>();

            DontDestroyOnLoad(gameObject);
        }

         private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player == null)
            {
                _playerRocket = null;
                _playerCollision = null;

                Debug.LogWarning("DebugManager: No Player found in the loaded scene.");
                return;
            }

            _playerRocket = player.transform;
            _playerCollision = _playerRocket.GetComponent<PlayerCollision>();
        }


        #endregion

        #region GameLoop
        // Update is called once per frame
        void Update()
        {
            DebugListener();
            ToggleCollisionListener();
        }

        #endregion

        #region Bahaviour 
        private void DebugListener()
        {
            if(Keyboard.current == null)
            {
                return; 
            }
            
            if (Keyboard.current.rKey.wasPressedThisFrame)
            {
                _sceneManagementService.ReloadScene();
            }

            if (Keyboard.current.nKey.wasPressedThisFrame)
            {
                _sceneManagementService.LoadNextScene();
            }

            if (Keyboard.current.lKey.wasPressedThisFrame)
            {
                _sceneManagementService.LoadPreviouseScene();
            }
        }

        private void ToggleCollisionListener()
        {
            if(Keyboard.current == null)
            {
                return; 
            }

            if (Keyboard.current.cKey.wasPressedThisFrame)
            {
                _playerCollision.SetCollidable();            
            }
        }
        #endregion
    }
}