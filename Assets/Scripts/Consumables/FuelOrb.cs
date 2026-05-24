using UnityEngine;

namespace Assets.Scripts.Consumables
{
    public class FuelOrb : MonoBehaviour
    {
        // Primatives
        private float _bounceHeight; 
        private float _bounceSpeed;
        private float _shrinkSpeed;
        private float _moveSpeed;

        private bool _isShrinking; 
        private Vector3 _startPosition;
        
        // Componenets
        private SphereCollider _sphereCollider; 
        private Transform _player;

        // Audio
        private AudioSource _audioSauce;
        private AudioClip _orbCollectionSfx;
        

        private void Awake()
        {
            SetFuleOrb();

            SetPlayer();

            SetAudio();
        }

        #region Game Loop 
        private void Update()
        {
            if (!_isShrinking)
            {
                Bounce();
            }
        }
        
        private void FixedUpdate()
        {

            if (_isShrinking)
            {
               Shrink();
            }
        }
        
        #endregion

        #region Setup 

        public void SetFuleOrb()
        {
            _bounceHeight = 0.1f;
            _bounceSpeed = 2.5f;
            _shrinkSpeed = 1f;
            _moveSpeed = 5f; 
            
            _isShrinking = false;
            
            _startPosition = transform.position;

            _sphereCollider = GetComponent<SphereCollider>();

            if (_sphereCollider == null){
                
                _sphereCollider = gameObject.AddComponent<SphereCollider>();
            }
            
            _sphereCollider.isTrigger = true; 
        }

        public void SetPlayer()
        {
            _player = GameObject.FindWithTag("Player").transform;
        }

        #endregion

        #region Audio & Visual

        public void SetAudio()
        {
            GameObject audio = new GameObject("AudioSource");
            
            audio.transform.SetParent(transform); 

            _audioSauce = audio.AddComponent<AudioSource>(); 

            _audioSauce.volume = 5f; 

            _orbCollectionSfx = Resources.Load<AudioClip>("Audio/Collectibles/OrbCollectionSfx");
        }

        #endregion

        #region Behaviour

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                _isShrinking = true;
                _sphereCollider.enabled = false;
                _audioSauce.PlayOneShot(_orbCollectionSfx); 
            }
        }

        private void Bounce()
        {
            float newY = _startPosition.y + Mathf.Sin(Time.time * _bounceSpeed) * _bounceHeight;
            transform.position = new Vector3(_startPosition.x, newY, _startPosition.z);
        }

        private void Shrink()
        {
            transform.position = Vector3.MoveTowards( transform.position, _player.transform.position, _moveSpeed * Time.fixedDeltaTime);
            transform.localScale -= Vector3.one * (_shrinkSpeed * Time.fixedDeltaTime);
                
            if (transform.localScale.x <= 0) {
                Destroy(gameObject);
            }   
        }

        #endregion
    }
}