using System.Collections;
using TMPro;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement")]
        public float walkSpeed;
        public float sprintSpeed;
        float _currentSpeed;
        float _horizontalInput;
        float _verticalInput;
        float _jump;

        public float groundDrag;

        public float jumpForce;
        public float jumpCooldown;
        public float airMultiplier;
        bool _readyToJump = true;
        bool _jumping = false;

        [Header("Ground Check")]
        public float playerHeight = 1f;
        public float playerToGroundDistance;
        bool _grounded;

        [Header("slope Handling")]
        float _maxSlopeAngle = 70;
        RaycastHit _slopeHit;

        [Header("Energy")]
        public float currentEnergy;
        float _energyTick = 0.1f;
        float _totalTick = 0.0f;
        bool _pauseEnergyUsage;
        float _maxEnergy = 2;

        [Header("TextUI")]
        public TextMeshProUGUI energyText;


        public Transform orientation;

        Rigidbody _rb;
        MeshCollider _collider;
        Vector3 _moveDirection;

        public MovementState state;
        private PlayerInputScript playerInput;
        public enum MovementState
        {
            walking,
            Sprinting
        }

        void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _collider = GetComponentInChildren<MeshCollider>();
            currentEnergy = _maxEnergy;
        }

        void Update()
        {
            _grounded = playerGroundCheck();
            ProcessInput();
            SpeedControl();
            if (_grounded)
                _rb.drag = groundDrag;
            else
                _rb.drag = 0;

            energyText.text = ((int)(currentEnergy * 50)).ToString();
        }
        
        bool playerGroundCheck()
        {
            float sphereCastRadius = _collider.bounds.extents.x * 0.9f;
            Debug.DrawRay(_rb.position, Vector3.down * playerHeight * 0.5f, Color.red);
            Physics.SphereCast(_rb.position, sphereCastRadius, Vector3.down, out RaycastHit _groundCheckHit);
            playerToGroundDistance = _groundCheckHit.distance + sphereCastRadius;
            return ((playerToGroundDistance >= _collider.bounds.extents.y - 0.1f)
                    && (playerToGroundDistance <= _collider.bounds.extents.y + 0.1f));
        }

        IEnumerator EnergyRegenerationTimer()
        {
            _pauseEnergyUsage = true;
            float timer = 0;
            while (timer < 5)
            {
                timer += Time.deltaTime;
                walking();
                yield return null;
            }
            _pauseEnergyUsage = false;
        }

        void stateHandler(float sprint, float jump)
        {
            if (currentEnergy <= 0)
            {
                StartCoroutine(EnergyRegenerationTimer());
            }
            else if (!_pauseEnergyUsage)
            {
                if (sprint != 0)
                {
                    state = MovementState.Sprinting;
                    _totalTick = Time.deltaTime * _energyTick;
                    currentEnergy -= _totalTick;

                    _currentSpeed = sprintSpeed;
                }
                else
                {
                    walking();
                }
                if (jump != 0 && _readyToJump && _grounded )
                {
                    currentEnergy -= 0.2f;
                    _readyToJump = false;
                    Jump();
                    StartCoroutine(JumpInProgress());
                }
            }
        }

        IEnumerator JumpInProgress()
        {
            _jumping = true;
            float timer = 0;
            while (timer < 1)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            resetJump();
            _jumping = false;
        }
        private void Awake()
        {
            playerInput = new PlayerInputScript();
            playerInput.Enable();
        }

        void walking()
        {
            if (currentEnergy < _maxEnergy)
            {
                _totalTick = Time.deltaTime * _energyTick;
                currentEnergy += _totalTick;
            }
            else
                currentEnergy = _maxEnergy;

            state = MovementState.walking;
            _currentSpeed = walkSpeed;
        }

        void FixedUpdate()
        {
            MovePlayer();
        }

        private void ProcessInput()
        {
            _horizontalInput = Input.GetAxis("Horizontal");
            _verticalInput = Input.GetAxis("Vertical");
            _jump = playerInput.FPSController.Jump.ReadValue<float>();

            float sprint = playerInput.FPSController.Sprint.ReadValue<float>();
            stateHandler(sprint, _jump);
        }

        private void MovePlayer()
        {
            _moveDirection = orientation.forward * _verticalInput + orientation.right * _horizontalInput;
            if (onSlope())
            {
                _rb.AddForce(getSlopeMoveDirection() * _currentSpeed * 25f, ForceMode.Force);
                if (_rb.velocity.y > 0 && _readyToJump && !_jumping)
                    _rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
            else
            if (_grounded)
                _rb.AddForce(_moveDirection.normalized * _currentSpeed * 10f, ForceMode.Force);
            else
                _rb.AddForce(_moveDirection.normalized * _currentSpeed * 10f * airMultiplier, ForceMode.Force);
        }

        private void SpeedControl()
        {
            Vector3 flatVel = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
            if (flatVel.magnitude > _currentSpeed)
            {
                Vector3 limitevel = flatVel.normalized * _currentSpeed;
                _rb.velocity = new Vector3(limitevel.x, _rb.velocity.y, limitevel.z);
            }
            Vector3 jumpVel = new Vector3(0f, _rb.velocity.y, 0f);
            if (jumpVel.y > 0)
            {
                if (jumpVel.y > jumpForce)
                    jumpVel.y = jumpForce;
                _rb.velocity = new Vector3(_rb.velocity.x, jumpVel.y, _rb.velocity.z);
            }
        }

        private void Jump()
        {
            _rb.velocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);

            _rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }

        private void resetJump()
        {
            _readyToJump = true;
        }

        private bool onSlope()
        {
            float sphereCastRadius = _collider.bounds.extents.x * 1f;
        
            if (Physics.SphereCast(_rb.position, sphereCastRadius, Vector3.down, out _slopeHit, playerHeight * 0.5f+0.5f))
            {
                float angle = Vector3.Angle(Vector3.up,_slopeHit.normal);
                return angle < _maxSlopeAngle && angle != 0;
            }
            return false;
        }

        private Vector3 getSlopeMoveDirection() {
            return Vector3.ProjectOnPlane(_moveDirection, _slopeHit.normal).normalized *1.2f; 
        }
    }
}
