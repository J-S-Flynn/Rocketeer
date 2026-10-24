using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Player {
    
    public class Movement : MonoBehaviour
    {

        // Primatives
        private float thrustStrength;
        private float thrustDecayRate;
        private float rotationStrength;
        private float _currentThrust;
        private float particalSizeLarge = 0.2f;
        private float particalSizeSmall = 0.05f;

        // components
        private Rigidbody _rocket;    
        private BoxCollider _boxCollider;

        // Audio  
        private AudioSource _audioSource;
        private AudioClip _thrustSfx; 
         
        // Partical 
        private ParticleSystem _rocketJetPfx;
        private ParticleSystem _leftThrustPfx;
        private ParticleSystem _rightThrustPfx;

        // Coroutines
        private Coroutine _fadeOutRoutine; 

        // Controlers 
        private  InputAction thrust;
        private InputAction rotation;

        // primatives   

        #region Init
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

        #endregion
        
        #region Game Loop
        //fixed update is far better when we are using physics. 
        private void FixedUpdate()
        {
            ProcessThrust();
            ProcessRotation();
        }

        #endregion

        #region Setup 
        
        public void SetRocketBody()
        {
            // Riged Body 
            _rocket = GetComponent<Rigidbody>(); 

            if(_rocket == null)
            {
                _rocket = gameObject.AddComponent<Rigidbody>();
            }

            _rocket.mass = 1f; 
            _rocket.linearDamping = 2.5f;
            _rocket.angularDamping = 5f; 

            _rocket.automaticCenterOfMass = true; 
            _rocket.automaticInertiaTensor = true; 
            _rocket.useGravity = true; 

            _rocket.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;

            // Box Collider 
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
            GameObject audio = new GameObject("AudioSouce"); 

            audio.transform.SetParent(transform); 

            _audioSource = audio.AddComponent<AudioSource>(); 

            _audioSource.volume = 0.5f;

            _thrustSfx = Resources.Load<AudioClip>("Audio/Rocket/ThrustSfx");
        }

        public void SetParticles()
        {
            // Load Jet 
            GameObject jetPfx = Resources.Load<GameObject>("Particles/JetPfx");

            GameObject jet = Instantiate(jetPfx, transform.position, Quaternion.identity);

            jet.transform.SetParent(transform);
            
            jet.transform.localPosition = new Vector3(0, -0.5f, 0);

            _rocketJetPfx = jet.GetComponent<ParticleSystem>();

            var jetShape = _rocketJetPfx.shape; 
            var jetMain = _rocketJetPfx.main; 
            var jetEmmision = _rocketJetPfx.emission;

            jetShape.scale = new Vector3(1f, 1.5f, 1f);
            jetMain.startSize = particalSizeLarge;
            jetEmmision.burstCount = 8;

            // Load Thruster 
            GameObject thrusterPfx = Resources.Load<GameObject>("Particles/ThrusterPfx");


            GameObject lThrust = Instantiate(thrusterPfx, transform.position, Quaternion.identity);
            GameObject rThrust = Instantiate(thrusterPfx, transform.position, Quaternion.identity);

            lThrust.transform.SetParent(transform);
            lThrust.transform.localPosition = new Vector3(0.75f, -0.25f, 0f);

            rThrust.transform.SetParent(transform);
            rThrust.transform.localPosition = new Vector3(-0.75f, -0.25f, 0f);

            _leftThrustPfx = lThrust.GetComponent<ParticleSystem>();

            var lShape = _leftThrustPfx.shape;
            var lMain = _leftThrustPfx.main;

            lShape.scale = new Vector3(0.5f, 0.25f, 0.1f);
            lMain.startSize = particalSizeSmall; 
            lMain.startSpeed = 1f;
            

            _rightThrustPfx = rThrust.GetComponent<ParticleSystem>();

            var rShape = _rightThrustPfx.shape;
            var rMain = _rightThrustPfx.main;

            rShape.scale = new Vector3(0.5f, 0.25f, 0.1f);
            rMain.startSize = particalSizeSmall; 
            rMain.startSpeed = -10f;
            
        }
                
        #endregion

        #region Thrust
        
        private void ProcessThrust()
        {
            if (thrust.IsPressed())
            {
                StartThrusters();
            }
            else
            {
                EndThrusting();
            }

            if (_currentThrust > 0f)
            {
                _rocket.AddRelativeForce(Vector3.up * (_currentThrust * Time.fixedDeltaTime));
            }
        }

        private void StartThrusters()
        {
            _rocket.AddRelativeForce(Vector3.up * (thrustStrength * Time.fixedDeltaTime));

            if (!_audioSource.isPlaying)
            {
                if (_fadeOutRoutine != null)
                {
                    StopCoroutine(_fadeOutRoutine);
                    _fadeOutRoutine = null;
                }

                _audioSource.PlayOneShot(_thrustSfx);
            }

            if (!_rocketJetPfx.isPlaying)
            {
                _rocketJetPfx.Play();
            }
        }

        private void EndThrusting()
        {
            _currentThrust = Mathf.MoveTowards(_currentThrust, 0f, thrustStrength * Time.fixedDeltaTime / thrustDecayRate);

            if (_audioSource.isPlaying && _fadeOutRoutine == null)
            {
                _fadeOutRoutine = StartCoroutine(FadeOutAudioVisuals());
            }

            _rocketJetPfx.Stop();
        }

        #endregion

        #region Rotation
        private void ProcessRotation()
        {
            var rotationValue = rotation.ReadValue<float>();

            if (rotationValue < 0f)
            {
                RotateRight();
            }
            else if (rotationValue > 0f)
            {
                RotateLeft();
            }
            else
            {
                StopRotation();
            }
        }

        private void RotateRight()
        {
            ApplyRotation(rotationStrength);

            if (!_leftThrustPfx.isPlaying)
            {
                _leftThrustPfx.Play();
                _audioSource.PlayOneShot(_thrustSfx);
            }
        }

        private void RotateLeft()
        {
            ApplyRotation(-rotationStrength);

            if (!_rightThrustPfx.isPlaying)
            {
                _rightThrustPfx.Play();
                _audioSource.PlayOneShot(_thrustSfx);
            }
        }

        private void StopRotation()
        {
            _leftThrustPfx.Stop();
            _rightThrustPfx.Stop();
        }

        private void ApplyRotation(float r)
        {
            transform.Rotate(Vector3.forward * (r * Time.fixedDeltaTime));
        }

        #endregion

        #region EndSequence
        
        private void OnDisable()
        {
            thrust.Disable();
            rotation.Disable(); 
        }

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