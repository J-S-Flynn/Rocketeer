
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Player
{
    public class PlayerCollision : MonoBehaviour
    {
        // Primatives
        private readonly float sceneDelay = 2f;
        private bool _controllable; 
     
        // Audio 
        private AudioSource _audioSource;
        private AudioClip _successSfx; 
        private AudioClip _crashSfx; 

        // Particals 
        private ParticleSystem _successPfx;
        private ParticleSystem _crashPfx;


        private void Start()
        {
            _controllable = true; 

            SetAudio();

            SetParticles();
        }

        #region Audio & Visual 

        public void SetAudio()
        {
            GameObject audio = new GameObject("AudioSource"); 

            audio.transform.SetParent(transform); 

            _audioSource = audio.AddComponent<AudioSource>(); 

            _audioSource.volume = 1f; 

            _crashSfx = Resources.Load<AudioClip>("Audio/Rocket/CrashSfx");  

            _successSfx = Resources.Load<AudioClip>("Audio/Rocket/SuccessSfx");   
            
        }

        public void SetParticles()
        {
            _successPfx = Resources.Load<ParticleSystem>("Particles/SuccessPfx");
            _crashPfx = Resources.Load<ParticleSystem>("Particles/CrashPfx"); 
        }

        #endregion

        #region Behaviour

        private void OnCollisionEnter(Collision other)
        {
            if (!_controllable) return; 
            
            switch (other.gameObject.tag)
            {
                case "Friendly":
                    break;
                case "Finish":
                    SuccessSequence();
                    break;
                default:
                    CrashSequence();
                    break;   
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            switch (other.gameObject.tag)
            {
                case "FuelOrb":
                    break;
                default:
                    break;  
            }
        }
        
        private void SuccessSequence()
        {
            
            DisableMovment();
            
            _audioSource.Stop();

            _audioSource.PlayOneShot(_successSfx);
            
            Instantiate(_successPfx, transform.position, Quaternion.identity).Play();
            
            Invoke("LoadNextScene", sceneDelay);
        }
        
        private void CrashSequence()
        {
            DisableMovment();
            
            _audioSource.Stop();

            _audioSource.PlayOneShot(_crashSfx);
            
            Instantiate(_crashPfx, transform.position, Quaternion.identity).Play();
            
            Invoke("ReloadScene", sceneDelay);  
        }
        
        private void DisableMovment()
        {
            _controllable = false;
            
            var movement = GetComponent<Movement>();
            
            movement.enabled = false;
        }
        
        #endregion

        #region Scene Loading
        
        private void LoadNextScene()
        {
            var currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            var lastSceneIndex = SceneManager.sceneCountInBuildSettings - 1;

            Debug.Log($"current {currentSceneIndex}" );
            Debug.Log($"last {lastSceneIndex}" );
            
            if (currentSceneIndex >= lastSceneIndex)
            {
                SceneManager.LoadScene(0);
            }
            else
            {
                SceneManager.LoadScene(currentSceneIndex + 1);
            }
        }
        
        private void ReloadScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        
        #endregion
    }
} 