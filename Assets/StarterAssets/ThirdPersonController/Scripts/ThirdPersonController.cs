using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : MonoBehaviour ,IDamageable
    {
        public float ChangedSpeed { private get; set; }
        [Header("Player")]
        [Tooltip("Sprint speed of the character in m/s")]
        public float DefaultSpeed = 5.335f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Space(10)]

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;
        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

        private DamageTypes damageType;
        public PlayerControllStates playerControllStates;
        public PlayerBuffs playerBuffs;
        [HideInInspector]public float CCtimer;
        public float delayAfterCancelTimer=0.2f;
        public CameraFollow cam;
        public bool speedChanged=false;
        public PlayerSO playerSO;
        public SkillQ skillQ;
        public SkillRMB skillRMB;
        public SkillLMB skillLMB;
        public SkillE skillE;
        public SkillSpace skillSpace;
        public SkillR skillR;
        public SkillF skillF;
        public SkillShiftSpace skillShiftSpace;
        public SkillShiftQ skillShiftQ;

        // player
        private float _speed;
        private float _animationBlend;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDMotionSpeed;

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        [HideInInspector] public Collider playerCollider;
        [HideInInspector] public Rigidbody rb;
        [HideInInspector] public StarterAssetsInputs _input;
        public GameObject playerGeometry;


        private bool _hasAnimator;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }

        private void Start()
        {
            _hasAnimator = TryGetComponent(out _animator);
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
            _playerInput = GetComponent<PlayerInput>();
            rb = GetComponent<Rigidbody>();
            playerCollider = GetComponent<Collider>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif
            AssignAnimationIDs();
        }
        private void FixedUpdate()
        {
            if (_input.canMove)
            {
                Move();
            }
            else
            {
                rb.velocity = Vector3.zero;
            }
        }
        public void TakeDamage(float amount,DamageTypes damageTypes,float debuffTime)
        {
            damageType = damageTypes;
            switch (damageType)
            {
                case DamageTypes.None:
                    break;
                case DamageTypes.Normal:
                    playerControllStates = PlayerControllStates.Normal;
                    playerSO.UpdateCurrentHp(-amount);
                    ClampHpValues();
                    CCtimer = debuffTime;

                    StartCoroutine(ClearDamageEffects(debuffTime));
                    ClampHpValues();
                    break;
                case DamageTypes.Stun:
                    playerControllStates = PlayerControllStates.Stunned;
                    playerSO.UpdateCurrentHp(-amount);
                    ClampHpValues();
                    CCtimer = debuffTime;
                    StartCoroutine(ClearDamageEffects(CCtimer));
                    break;
            }
        }
        public IEnumerator ClearDamageEffects(float time)
        {
            switch (playerControllStates)
            {
                case PlayerControllStates.None:
                    speedChanged = false;
                    break;
                case PlayerControllStates.Stunned:
                    ChangedSpeed = 0;
                    _speed = 0;
                    speedChanged = true;
                    yield return new WaitForSeconds(time);
                    playerControllStates = PlayerControllStates.None;
                    speedChanged = false;
                    CCtimer = 0;
                    break;
                case PlayerControllStates.Silenced:
                    yield return new WaitForSeconds(time);
                    playerControllStates = PlayerControllStates.None;
                    break;
                case PlayerControllStates.Slowed:
                    speedChanged = true;
                    yield return new WaitForSeconds(time);
                    playerControllStates = PlayerControllStates.None;
                    speedChanged = false;
                    CCtimer = 0;
                    break;
                case PlayerControllStates.DamagedOvertime:
                    yield return new WaitForSeconds(time);
                    playerControllStates = PlayerControllStates.None;
                    break;
            }
        }
        public IEnumerator GetBuffedEffects(float time)
        {
            switch (playerBuffs)
            {
                case PlayerBuffs.None:
                    break;
                case PlayerBuffs.Normal:
                    yield return new WaitForSeconds(time);
                    playerControllStates = PlayerControllStates.None;
                    break;
                case PlayerBuffs.SpeedUp:
                    yield return new WaitForSeconds(time);
                    playerControllStates = PlayerControllStates.None;
                    break;
                case PlayerBuffs.LifeSteal:
                    skillLMB.bulletType = DamageTypes.LifeSteal;
                    yield return new WaitForSeconds(time);
                    playerControllStates = PlayerControllStates.None;
                    skillLMB.bulletType = DamageTypes.Normal;
                    break;
                case PlayerBuffs.AtkBoost:
                    yield return new WaitForSeconds(time);
                    playerControllStates = PlayerControllStates.None;
                    break;
                case PlayerBuffs.DamageReduction:
                    yield return new WaitForSeconds(time);
                    playerControllStates = PlayerControllStates.None;
                    break;
                default:
                    break;
            }
        }
        private void Update()
        {
            Cooldown();
            _hasAnimator = TryGetComponent(out _animator);
            cam.ZoomCam(-_input.mouseScrollY);
            LockCam();
            if (_input.delayAfterCancel)
            {
                StartCoroutine(CancelDelay());
            }
            
        }
        public IEnumerator CancelDelay()
        {
            yield return new WaitForSeconds(delayAfterCancelTimer);
            _input.delayAfterCancel = false;
        }
        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }
        private void Move()
        {
            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = speedChanged ? ChangedSpeed : DefaultSpeed;

            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (_speed < targetSpeed - speedOffset)
            {
                if (_speed < 1)
                {
                    _speed = 1;
                }
                _speed += SpeedChangeRate * Time.deltaTime;
                _speed = Mathf.Clamp(_speed, 0, targetSpeed);
            }
            else if (_speed > targetSpeed + speedOffset)
            {
                _speed -= SpeedChangeRate * Time.deltaTime;
                _speed = Mathf.Clamp(_speed, targetSpeed, _speed);
            }
            else
            {
                _speed = targetSpeed;
            }
            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            // normalise input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;
            Vector3 movement = new Vector3(_input.move.x, 0.0f, _input.move.y);
            movement.Normalize();
            movement *= _speed * Time.deltaTime;
            float velocityZ = Vector3.Dot(movement.normalized, transform.forward);
            float velocityX = Vector3.Dot(movement.normalized, transform.right);
            //float velocityX = Mathf.Clamp(_input.move.x, -1, 1);
            //float velocityZ = Mathf.Clamp(_input.move.y,-1,1);
            _animator.SetFloat("VelocityZ", velocityZ, 0.1f, Time.deltaTime);
            _animator.SetFloat("VelocityX", velocityX, 0.1f, Time.deltaTime);

            // move the player
            //rb.MovePosition(transform.position + inputDirection.normalized * (_speed * Time.deltaTime));
            rb.velocity = new Vector3(inputDirection.normalized.x * _speed , 0, inputDirection.normalized.z * _speed );
            // update animator if using character
           //if (_hasAnimator)
           //{
           //    _animator.SetFloat(_animIDSpeed, _animationBlend);
           //    _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
           //}
        }
        private void LockCam()
        {
            cam.camLocked= _input.lockCam ? true : false;
        }
        public void Cooldown()
        {
            if (skillQ.onCooldown && skillQ.coolDownCounter!=0)
            {
                skillQ.coolDownCounter -= Time.deltaTime;
                skillQ.coolDownCounter = Mathf.Clamp(skillQ.coolDownCounter, 0, skillQ.coolDown);
            }
            else
            {
                skillQ.coolDownCounter = skillQ.coolDown;
                skillQ.onCooldown = false;

            }
            if (skillShiftQ.onCooldown && skillShiftQ.coolDownCounter != 0)
            {
                skillShiftQ.coolDownCounter -= Time.deltaTime;
                skillShiftQ.coolDownCounter = Mathf.Clamp(skillShiftQ.coolDownCounter, 0, skillShiftQ.coolDown);
            }
            else
            {
                skillShiftQ.coolDownCounter = skillShiftQ.coolDown;
                skillShiftQ.onCooldown = false;
            }
        }
        public void UpdateActionProgress(float maxValue)
        {
            playerSO.UpdateActionProgress(maxValue);
        }
        private void ClampHpValues()
        {
            if (playerSO.currentMaxHp-playerSO.currentHp>=40)
            {
                playerSO.UpdateCurrentMaxHp(playerSO.currentHp+40);
            }
        }
        
        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }
        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(transform.position), FootstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(transform.position), FootstepAudioVolume);
            }
        }
        
    }
}