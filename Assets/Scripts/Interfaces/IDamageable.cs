using UnityEngine;

namespace Interfaces
{
    public interface IDamageable 
    {
        public int CurrentHealth { get; }
        public int MaxHealth { get; }

        public delegate void TakeDamageEvent(int damage);
        public event TakeDamageEvent OnTakeDamage;

        public delegate void DieEvent(Vector3 Position);
        public event DieEvent OnDeath;

        public void TakeDamage(int damage);

    }
}