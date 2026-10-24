using UnityEngine;
using Assets.Scripts.Services;


namespace Assets.Scripts.Player
{
    public class PlayerCollision : MonoBehaviour
    {

        //Imports
        private SceneManagementService _sceneManagementService;

        // Primatives
        private float ParticleSize = 0.25f; 
        private bool _collidable; 
        private bool _controllable;

        // Audio 
        private AudioSource _audioSource;
        private AudioClip _successSfx; 
        private AudioClip _crashSfx; 

        // Particals 
        private ParticleSystem _successPfx;
        private ParticleSystem _crashPfx;

        #region Init
        
        private void Awake()
        {
            _collidable = true;
            _controllable = true;

            _sceneManagementService = SceneManagementService.Instance;
        }

        private void Start()
        {  
            SetAudio();

            SetParticles();
        }

        #endregion

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

            ParticleSystem.MainModule successMain = _successPfx.main; 
            successMain.startSize = ParticleSize;

            _crashPfx = Resources.Load<ParticleSystem>("Particles/CrashPfx"); 

            ParticleSystem.MainModule crashMain = _crashPfx.main;
            crashMain.startSize = ParticleSize; 
        }

        #endregion

        #region Behaviour

        private void OnCollisionEnter(Collision other)
        {
            if (!_collidable) return;
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
            
            Invoke(nameof(LoadNextScene), 2f);
        }
        
        private void CrashSequence()
        {
            DisableMovment();
            
            _audioSource.Stop();

            _audioSource.PlayOneShot(_crashSfx);
            
            Instantiate(_crashPfx, transform.position, Quaternion.identity).Play();
            
            Invoke(nameof(ReloadScene), 2f);
        }
        
        private void DisableMovment()
        {
            _controllable = false;
            
            var movement = GetComponent<Movement>();
            
            movement.enabled = false;
        }
        
        #endregion


        #region public Accessors
      
        private void LoadNextScene()
        {
            _sceneManagementService.LoadNextScene();
        }

        private void ReloadScene()
        {
            _sceneManagementService.ReloadScene();  
        }
        
        public void SetCollidable()
        {
            _collidable = !_collidable; 
        }
        
        #endregion
    }
} 