using Health;
using Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class HealthPerk : Interactable
{
    public AudioClip audioClip;

    private GameObject _player;
    private PlayerHealth _playerHealth;
    private bool _isPickedUp = false;
    private AudioSource _audioSource;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("PlayerObject");
        _playerHealth = _player.GetComponent<PlayerHealth>();
        _audioSource = GetComponent<AudioSource>();
    }

    protected override void Interact()
    {
        if (!_isPickedUp)
        {
            _isPickedUp = true;
            _playerHealth.IncreaseHealthPerk();

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
