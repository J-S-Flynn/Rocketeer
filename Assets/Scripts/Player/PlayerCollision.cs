
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Player
{
    public class PlayerCollision : MonoBehaviour
    {
        // Serialisable fields
        private readonly float sceneDelay = 2f;

        // Components
        private GameObject _audio;
     
        //Audio 
        private AudioSource _audioSource;
        private AudioClip _crashSfx;
        private AudioClip _successSfx;

        // Particals 
        private ParticleSystem _successPfx;
        private ParticleSystem _crashPfx;

        // Primatives 
        private bool _controllable; 
        
        

        private void Start()
        {
            // Create and attach Audio Object 
            _audio = new GameObject("AudioSouce"); 
            _audio.transform.SetParent(transform); 

            _audioSource = _audio.AddComponent<AudioSource>(); 

            _audioSource.volume = 1f;
            
            _controllable = true;

            _successSfx = Resources.Load<AudioClip>("Audio/Rocket/SuccessSfx");
            _crashSfx = Resources.Load<AudioClip>("Audio/Rocket/CrashSfx");
            

            _successPfx = Resources.Load<ParticleSystem>("Particles/SuccessPfx");
            _crashPfx = Resources.Load<ParticleSystem>("Particles/CrashPfx"); 
        }

        #region Audio & Visual 

        public void SetAudio()
        {
            
        }

        public void SetParticles()
        {
            
        }

        #endregion

        #region collisions and triggers

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
        
        #endregion
    
        #region StopSequences

        private void SuccessSequence()
        {
            // TODO add sfx and particles
            
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

        #region SceneLoading
        
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