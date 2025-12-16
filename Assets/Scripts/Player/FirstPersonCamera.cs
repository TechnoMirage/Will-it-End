using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class FirstPersonCamera : MonoBehaviour
    {
        private PlayerInputScript playerInput;
        public float sensX = 100f;
        public float sensY = 100f;
        public Transform PlayerOrientation;

        private float xRotation;
        private float yRotation;

        private void Awake()
        {
           playerInput = new PlayerInputScript();
        }

        void Start()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        bool IsUsingM2ouseAndKeyboard()
        {
            string[] joystickNames = Input.GetJoystickNames();
            return joystickNames.Length == 0;
        }

        private void OnEnable()
        {
            playerInput.Enable();
        }

        private void DoLooking()
        {
            Vector2 look = playerInput.FPSController.Look.ReadValue<Vector2>();
            float inputX = look.x * sensX * Time.deltaTime;
            float inputY = look.y * sensY * Time.deltaTime;
            xRotation += -look.y * sensY * Time.deltaTime;
            yRotation += look.x * sensX * Time.deltaTime;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
            PlayerOrientation.rotation = Quaternion.Euler(0, yRotation, 0);
        }

        public Vector2 GetPlayerLook()
        {
            return playerInput.FPSController.Look.ReadValue<Vector2>();
        }

        void Update()
        {
            DoLooking();
        }
    }
}
