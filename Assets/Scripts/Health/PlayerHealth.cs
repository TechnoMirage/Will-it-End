using Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Health
{
    public class PlayerHealth : MonoBehaviour, IDamageable
    {
        private TextMeshProUGUI healthText;
        private int _MaxHealth = 100;

        public int CurrentHealth { get; private set; }

        public int MaxHealth { get => _MaxHealth; private set => _MaxHealth = value; }
    
        public event IDamageable.TakeDamageEvent OnTakeDamage;
        public event IDamageable.DieEvent OnDeath;
        void Start()
        {
            healthText = GameObject.Find("HealthCount").GetComponent<TextMeshProUGUI>();
            CurrentHealth = MaxHealth;
        
        }

        // Update is called once per frame
        void Update()
        {
            healthText.text = CurrentHealth.ToString();

            if (Input.GetKeyDown(KeyCode.K))
            {
                TakeDamage(10);
            }
        }

        public void TakeDamage(int damage)
        {
            int damageTaken = Mathf.Clamp(damage, 0, CurrentHealth);
            CurrentHealth -= damageTaken;
        
            if (CurrentHealth <= 0)
            {
                SceneManager.LoadScene("DeathMenu");
            }
            else if (damageTaken != 0)
            {
                OnTakeDamage?.Invoke(damageTaken);
            }
        }

        public void Heal()
        {
            int middleHealth = MaxHealth / 2;

            if (CurrentHealth + middleHealth > MaxHealth)
            {
                CurrentHealth = MaxHealth;
                return;
            }

            CurrentHealth += middleHealth;
        }

        public void IncreaseHealthPerk()
        {
            MaxHealth += 25;
            CurrentHealth += 25;
        }
    }
}
