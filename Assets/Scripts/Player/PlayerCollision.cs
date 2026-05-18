using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Player
{
    public class PlayerCollision : MonoBehaviour
    {
        [SerializeField] private float sceneDelay = 2f;

        private ParticleSystem _successPfx; 
        private ParticleSystem _crashPfx;
        
        private AudioSource _audioSource;
        
        private AudioClip _crashSfx;
        private AudioClip _successSfx;

        private bool _controllable; 
        
        #region collisions and triggers

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.volume = 1f;
            
            _controllable = true;

            _successPfx = Resources.Load<ParticleSystem>("Particles/SuccessPfx");
            _crashPfx = Resources.Load<ParticleSystem>("Particles/CrashPfx");
            
            _successSfx = Resources.Load<AudioClip>("Audio/Rocket/successSfx");
            _crashSfx = Resources.Load<AudioClip>("Audio/Rocket/crasheSfx");
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!_controllable) return; 
            
            switch (other.gameObject.tag)
            {
                case "Friendly":
                    break;
                case "Finish":
                    StartSuccessSequence();
                    break;
                default:
                    StartCrashSequence();
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

        private void StartSuccessSequence()
        {
            // TODO add sfx and particles
            
            DisableMovment();
            
            _audioSource.Stop();
            _audioSource.PlayOneShot(_successSfx);
            
            Instantiate(_successPfx, transform.position, Quaternion.identity).Play();
            
            Invoke("LoadNextScene", sceneDelay);
        }
        
        private void StartCrashSequence()
        {
            // TODO add sfx and particles 
            
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