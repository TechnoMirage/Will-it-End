using Gun.Gun;
using Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class AmmoPerk : Interactable
{
    public GunDatabase gunDatabase;
    public AudioClip audioClip;

    private bool _isPickedUp = false;
    private InventoryManager _inventoryManager;
    private AudioSource _audioSource;

    private void Start()
    {
        _inventoryManager = FindObjectOfType<InventoryManager>();
        _audioSource = GetComponent<AudioSource>();
    }

    protected override void Interact()
    {
        if (!_isPickedUp)
        {
            if (gunDatabase == null)
            {
                return;
            }

            _isPickedUp = true;
            gunDatabase.guns.ForEach(gun =>
            {
                gun.AmmoConfig.MaxAmmo += gun.AmmoConfig.DefaultClipSize * 2;
                gun.AmmoConfig.ClipSize += gun.AmmoConfig.ClipSize;
            });

            _audioSource.clip = audioClip;
            _audioSource.Play();

            HideObject();

            _inventoryManager.RefreshGuns();
            Destroy(gameObject, audioClip.length);
        }
    }

    private void HideObject()
    {
        gameObject.GetComponent<Collider>().enabled = false;

        foreach (var child in GetComponentsInChildren<MeshRenderer>())
        {
            child.enabled = false;
        }
    }
}
