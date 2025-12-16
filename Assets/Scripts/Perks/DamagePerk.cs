using Gun.Gun;
using Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class DamagePerk : Interactable
{
    public GunDatabase gunDatabase;
    public AudioClip audioClip;

    private AudioSource _audioSource;
    private bool _isPickedUp = false;

    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    protected override void Interact()
    {
        if (!_isPickedUp)
        {
            _isPickedUp = true;
            gunDatabase.guns.ForEach(gun =>
            {
                gun.DamageConfig.DamageRange = new Vector2Int(gun.DamageConfig.DamageRange.x + 5, gun.DamageConfig.DamageRange.y + 5);
            });

            _audioSource.clip = audioClip;
            _audioSource.Play();

            HideObject();

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
