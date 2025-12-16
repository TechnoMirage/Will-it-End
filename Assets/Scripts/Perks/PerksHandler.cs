using Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerksHandler : Interactable
{
    public List<GameObject> perks;
    public AudioClip deniedSound;

    private GameObject _bottleSpawn;
    private HandleRounds _handleRounds;
    private AudioSource _audioSource;
    private int _previousRound;
    private bool _hasBeenCollectedInCurrentRound = false;

    // Start is called before the first frame update
    void Start()
    {
        _bottleSpawn = gameObject.transform.Find("BottleSpawner").gameObject;
        _handleRounds = GameObject.Find("RoundHandler").GetComponent<HandleRounds>();
        _audioSource = GetComponent<AudioSource>();
        _previousRound = _handleRounds.getCurrentRound();
    }

    protected override void Interact()
    {
        if (!_handleRounds.checkIfMachineShouldSpawnPerk())
        {
            _audioSource.clip = deniedSound;
            _audioSource.Play();
            return;
        }

        if (_previousRound != _handleRounds.getCurrentRound())
        {
            _hasBeenCollectedInCurrentRound = false;
        }

        if (_hasBeenCollectedInCurrentRound == false)
        {
            _previousRound = _handleRounds.getCurrentRound();
            _hasBeenCollectedInCurrentRound = true;
            int randomBottle = Random.Range(0, perks.Count);
            Instantiate(perks[randomBottle], new Vector3(_bottleSpawn.transform.position.x, _bottleSpawn.transform.position.y + 0.1f, _bottleSpawn.transform.position.z), Quaternion.identity);
        }
        else
        {
            _audioSource.clip = deniedSound;
            _audioSource.Play();
        }
    }
}
