using Interfaces;
using UnityEngine;

public class ZombieHealth: MonoBehaviour, IDamageable
{
    private int _MaxHealth = 100;
    [SerializeField]
    private int _Health;
    public int CurrentHealth { get => _Health; private set => _Health = value;}

    public int MaxHealth { get => _MaxHealth; private set => _MaxHealth = value; }

    public event IDamageable.TakeDamageEvent OnTakeDamage;
    public event IDamageable.DieEvent OnDeath;

    private void OnEnable()
    {
        CurrentHealth = MaxHealth;
    }

    public void TakeDamage(int damage)
    {
        int damageTaken = Mathf.Clamp(damage,0,CurrentHealth);

        CurrentHealth -= damageTaken;

        if(damageTaken != 0)
        {
            OnTakeDamage?.Invoke(damageTaken);
        }

        if(CurrentHealth == 0 && damageTaken != 0)
        {
            OnDeath?.Invoke(transform.position);
        }
    }

    public void IncreaseHealth(int amount)
    {
        MaxHealth += amount;
        CurrentHealth = MaxHealth;
    }
}
