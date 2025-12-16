using Gun.Bullet;
using Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoController : Interactable
{
    public AmmoScriptableObject ammoConfig;
    public AudioClip pickupSound;

    private InventoryManager inventoryManager;
    private bool isPickedUp = false;
    private AudioSource audioSource;

    protected override void Interact()
    {
        if (!isPickedUp)
        {
            isPickedUp = true;

            if (ammoConfig.MaxAmmo > ammoConfig.DefaultMaxAmmo)
            {
                if (ammoConfig.CurrentAmmo + ammoConfig.DefaultMaxAmmo > ammoConfig.MaxAmmo)
                {
                    ammoConfig.CurrentAmmo = ammoConfig.MaxAmmo;
                }
                else
                {
                    ammoConfig.CurrentAmmo += ammoConfig.DefaultMaxAmmo;
                }
            }
            else
            {
                ammoConfig.CurrentAmmo = ammoConfig.DefaultMaxAmmo;
            }

            inventoryManager.RefreshGuns();
            audioSource.clip = pickupSound;
            audioSource.Play();
            Destroy(gameObject, 0.2f);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        inventoryManager = FindObjectOfType<InventoryManager>();
        audioSource = GetComponent<AudioSource>();
    }
}
