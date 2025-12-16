using Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{

    public Animator Animator;
    public ZombieHealth Health;
    public ZombieController Movement;
    public NavMeshAgent Agent; 

    private AudioSource audioSource;
    public AudioClip zombieSound;
    public AudioClip zombieSoundDead;

    private bool soundPlayed = false;
    private void Start()
    {
        Health.OnDeath += Die;
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = zombieSound;

        StartCoroutine(PlayRandomSound());
    }

    private void Die(Vector3 position)
    {
        audioSource.clip = zombieSoundDead;
        audioSource.Play();

        Movement.HandleStateChange(EnemyState.Dead);
    }

    IEnumerator PlayRandomSound()
    {
        float waitTime = Random.Range(1f, 3f);
        yield return new WaitForSeconds(waitTime);

        if (!soundPlayed)
        {
            audioSource.Play();
            soundPlayed = true;
        }
    }
}
