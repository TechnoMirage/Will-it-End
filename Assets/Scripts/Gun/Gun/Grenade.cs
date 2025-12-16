using Gun.Gun;
using Interfaces;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class Grenade : Interactable
{
    public float delay = 3f;
    public float blastRadius = 5f;

    public bool shouldExplode = false;
    
    public GameObject explosionEffect;
    public DamageConfigScriptableObject DamageConfig;
    public AudioClip explosionSound;
    public AudioClip pickupSound;

    AudioSource audioSource;
    InventoryManager inventoryManager;
    float countdown;
    bool hasExploded = false;
    bool hasBeenPickedUp = false;

    // Start is called before the first frame update
    void Start()
    {
        countdown = delay;
        audioSource = GetComponent<AudioSource>();

        inventoryManager = FindObjectOfType<InventoryManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldExplode)
        {
            countdown -= Time.deltaTime;
            if (countdown <= 0f && hasExploded == false)
            {
                Explode();
                hasExploded = true;
            }
        }
    }

    void Explode()
    {
        audioSource.clip = explosionSound;
        audioSource.Play();
        GameObject explosion = Instantiate(explosionEffect, transform.position, transform.rotation);
        Destroy(explosion, 1.95f);

        Collider[] colliders = Physics.OverlapSphere(transform.position, blastRadius);

        foreach (Collider nearbyObject in colliders)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(1000, transform.position, blastRadius);
            }

            if (nearbyObject.gameObject.layer==3)
            {
                Transform currentObject = nearbyObject.transform;
                while (currentObject.TryGetComponent(out IDamageable damageableComponent) == false)
                {
                    currentObject = currentObject.parent;
                }
                currentObject.GetComponentInParent<IDamageable>()?.TakeDamage(DamageConfig.GetDamage());
            }
            else if (nearbyObject.TryGetComponent(out IDamageable damageable))
            {
                 damageable.TakeDamage(DamageConfig.GetDamage());
            }
        }

        Destroy(gameObject, 0.5f);
    }

    // It is being called from PlayerAction.cs using SendMessage - Gab
    public void ChangeExplosionType()
    {
        shouldExplode = true;
    }

    protected override void Interact()
    {
        if (hasBeenPickedUp)
        {
            return;
        }

        if (shouldExplode)
        {
            return;
        }

        if (inventoryManager.AddGrenade(1))
        {
            audioSource.clip = pickupSound;
            audioSource.Play();
            hasBeenPickedUp = true;
            Destroy(gameObject, 0.2f);
        }
    }
}
