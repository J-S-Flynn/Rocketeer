using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Player {
    
    public class Movement : MonoBehaviour
    {
        [SerializeField] private InputAction thrust;
        [SerializeField] private InputAction rotation;
        [SerializeField] private float thrustStrength;
        [SerializeField] private float thrustDecayRate;
        [SerializeField] private float rotationStrength;
        
        private AudioSource _audioSource;
        
        private AudioClip _thrustSfx; 
        
        private float _currentThrust;

        private Rigidbody _rigidbody;
        private Coroutine _fadeOutRoutine; 
        

        private void Start()
        {
            
            _audioSource  = GetComponent<AudioSource>();
            _rigidbody = GetComponent<Rigidbody>();
            
            _thrustSfx = Resources.Load<AudioClip>("Audio/Rocket/thrustSfx");

            thrustStrength = 750f; 
            rotationStrength = 250f;
            thrustDecayRate = 2f;
        }

        //fixed update is far better when we are using physics. 
        private void FixedUpdate()
        {
            ProcessThrust();
            ProcessRotation();
        }

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
            }
            else
            {
                _currentThrust = Mathf.MoveTowards(_currentThrust, 0f, thrustStrength * Time.fixedDeltaTime / thrustDecayRate);

                if (_audioSource.isPlaying && _fadeOutRoutine == null)
                {
                    _fadeOutRoutine = StartCoroutine(FadeOutAudio());
                }
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
            _rigidbody.freezeRotation = true;

            transform.Rotate(Vector3.forward * (r * Time.fixedDeltaTime));

            _rigidbody.freezeRotation = false;
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

        private void OnDisable()
        {
            thrust.Disable();
            rotation.Disable(); 
        }

        #endregion

        #region Sound

        private IEnumerator FadeOutAudio()
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