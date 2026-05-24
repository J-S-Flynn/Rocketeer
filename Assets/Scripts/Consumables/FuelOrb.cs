using UnityEngine;

namespace Consumables
{
    public class FuelOrb : MonoBehaviour
    {
        private float _bounceHeight; 
        private float _bounceSpeed;
        private float _shrinkSpeed;
        private float _moveSpeed;

        private bool _isShrinking; 
        
        private Vector3 _startPosition;
        private Transform _player;
        private Collider _collider;

        private void Awake()
        {
            _bounceHeight = 0.1f;
            _bounceSpeed = 2.5f;
            _shrinkSpeed = 1f;
            _moveSpeed = 5f; 
            
            _isShrinking = false;
            
            _startPosition = transform.position;
            _collider = GetComponent<Collider>();
            
            _collider.isTrigger = true; 
            _player = GameObject.FindWithTag("Player").transform;
        }

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
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                _isShrinking = true;
                _collider.enabled = false;
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
    }
}