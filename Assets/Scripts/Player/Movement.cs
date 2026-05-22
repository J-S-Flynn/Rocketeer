using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

namespace Player {
    
    public class Movement : MonoBehaviour
    {
        // Primatives
        private float thrustStrength;
        private float thrustDecayRate;
        private float rotationStrength;
        private float _currentThrust;

        // components
        private Rigidbody _rigidbody;    
        private BoxCollider _boxCollider;

        // Audio  
        private AudioSource _audioSource;
        private AudioClip _thrustSfx; 
         
        // Partical 
        private ParticleSystem _rocketJetPfx;
        // private ParticleSystem _leftThrustPfx;
        // private ParticleSystem _rightThrustPfx;

        // Coroutines
        private Coroutine _fadeOutRoutine; 

        // Controlers 
        private  InputAction thrust;
        private InputAction rotation;

        // primatives   

        private void Start()
        {
            // components
            SetRocketBody();

            SetAudio();
    
            SetParticles();

            thrustStrength = 750f; 
            rotationStrength = 250f;
            thrustDecayRate = 2f;
        }

        private void OnEnable()  
        {
            // Actions 
            thrust = new InputAction(type: InputActionType.Button);
            rotation = new InputAction(type: InputActionType.Button);

            // Keyboard
            thrust.AddBinding("<Keyboard>/space");

            rotation.AddCompositeBinding("1DAxis")
                .With("Negative", "<Keyboard>/leftArrow")
                .With("Positive", "<Keyboard>/rightArrow");


            // Game Pad 
            thrust.AddBinding("<Gamepad>/buttonSouth");

            rotation.AddCompositeBinding("1DAxis")
                .With("Negative", "<Gamepad>/dpad/left")
                .With("Positive", "<Gamepad>/dpad/right");


            thrust.Enable();
            rotation.Enable();
        }
        
        //fixed update is far better when we are using physics. 
        private void FixedUpdate()
        {
            ProcessThrust();
            ProcessRotation();
        }

        #region Body 
        
        public void SetRocketBody()
        {
            // Riged Body 
            _rigidbody = GetComponent<Rigidbody>(); 

            if(_rigidbody == null)
            {
                _rigidbody = gameObject.AddComponent<Rigidbody>();
            }

            _rigidbody.mass = 1f; 
            _rigidbody.linearDamping = 2.5f;
            _rigidbody.angularDamping = 5f; 

            _rigidbody.automaticCenterOfMass = true; 
            _rigidbody.automaticInertiaTensor = true; 
            _rigidbody.useGravity = true; 

            _rigidbody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;

            // Boc Collider 
            _boxCollider = GetComponent<BoxCollider>(); 

            if(_boxCollider == null)
            {
                _boxCollider = gameObject.AddComponent<BoxCollider>();
            }

            _boxCollider.size = new Vector3(1, 1, 0.5f);  

        }
        
        #endregion

        #region Audio & Visual
        public void SetAudio()
        {
             // Create and attach Audio Object 
            GameObject _audio = new GameObject("AudioSouce"); 
            _audio.transform.SetParent(transform); 

            _audioSource = _audio.AddComponent<AudioSource>(); 

            // Load Audio
            _thrustSfx = Resources.Load<AudioClip>("Audio/Rocket/ThrustSfx");
        }

        public void SetParticles()
        {
            // Load and add Particles System   
            GameObject jetPfx = Resources.Load<GameObject>("Particles/JetPfx");
            GameObject jet = Instantiate(jetPfx, transform.position, Quaternion.identity);

            jet.transform.SetParent(transform);
            jet.transform.localPosition = new Vector3(0, -0.5f, 0);

            _rocketJetPfx = jet.GetComponent<ParticleSystem>();

            
        }

        #endregion

        #region Movment
 
        
        private void ProcessThrust()
        {
            if (thrust.IsPressed())
            {

                _rigidbody.AddRelativeForce(Vector3.up * (thrustStrength * Time.fixedDeltaTime));
                
                if (!_audioSource.isPlaying)
                {
                    if (_fadeOutRoutine != null)
                    {
                        StopCoroutine(_fadeOutRoutine);
                        _fadeOutRoutine = null;
                    }
                    
                    _audioSource.PlayOneShot(_thrustSfx);
                }

                if (!_rocketJetPfx.isEmitting)
                {
                    _rocketJetPfx.Play(); 
                }

            }
            else
            {
                _currentThrust = Mathf.MoveTowards(_currentThrust, 0f, thrustStrength * Time.fixedDeltaTime / thrustDecayRate);

                if (_audioSource.isPlaying && _fadeOutRoutine == null)
                {
                    _fadeOutRoutine = StartCoroutine(FadeOutAudioVisuals());
                }

                _rocketJetPfx.Stop();
            } 
            
            if (_currentThrust > 0f)
            {
                _rigidbody.AddRelativeForce(Vector3.up * (_currentThrust * Time.fixedDeltaTime));
            }
        }

        private void ProcessRotation()
        {
            var rotationValue = rotation.ReadValue<float>();

            if (rotationValue < 0f)
            {
                ApplyRotation(rotationStrength);
            }
            else if (rotationValue > 0f)
            {
                ApplyRotation(-rotationStrength);
            }
        }

        private void ApplyRotation(float r)
        {

            transform.Rotate(Vector3.forward * (r * Time.fixedDeltaTime));
        }

        private void OnDisable()
        {
            thrust.Disable();
            rotation.Disable(); 
        }

        #endregion

        #region Sound

        private IEnumerator FadeOutAudioVisuals()
        {
            float fadeSpeed = 1f; // seconds to fade out (adjust to taste)

            while (_audioSource.volume > 0.01f)
            {
                _audioSource.volume -= Time.deltaTime / fadeSpeed;
                yield return null;
            } 

            _audioSource.Stop();
            _audioSource.volume = 1f; // reset for next use
            _fadeOutRoutine = null;
        }

        #endregion
    }
}