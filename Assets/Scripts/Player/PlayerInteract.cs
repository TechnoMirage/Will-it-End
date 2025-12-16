using System;
using Interfaces;
using UnityEngine;

namespace Player
{
    public class PlayerInteract : MonoBehaviour
    {
        public Camera _playerCamera;
        private float _interactDistance = 3f;
        [SerializeField]
        private LayerMask LayerMask;
        private PlayerInputScript playerInput;
        private GameObject interactCanvas;

        void Start()
        {
            interactCanvas = GameObject.Find("InteractCanvas");
            interactCanvas.SetActive(!interactCanvas.activeSelf);
        }

        private void Awake()
        {
            playerInput = new PlayerInputScript();
            playerInput = new PlayerInputScript();
            playerInput.Enable();
        }

        void Update()
        {
            Ray ray = new Ray(_playerCamera.transform.position, _playerCamera.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, _interactDistance, LayerMask))
            {
                if (hit.collider.gameObject.GetComponent<Interactable>() != null)
                {
                    if (!GrenadeCheckup(hit))
                    {
                        interactCanvas.SetActive(true);
                    }

                    if (playerInput.FPSController.Interact.triggered)
                    {
                        hit.collider.GetComponent<Interactable>().baseInteract();
                    } 
                }
            }

            if (!Physics.Raycast(ray, out hit, _interactDistance, LayerMask))
            {
                interactCanvas.SetActive(false);
            }
        }

        bool GrenadeCheckup(RaycastHit hit)
        {
            if (hit.collider.gameObject.GetComponent<Grenade>() != null)
            {
                if (hit.collider.gameObject.GetComponent<Grenade>().shouldExplode == true)
                    interactCanvas.SetActive(false);
                else
                    interactCanvas.SetActive(true);

                return true;
            }

            return false;
        }
    }
}
