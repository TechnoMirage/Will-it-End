using Interfaces;
using System.Collections;
using System.Collections.Generic;
using Health;
using UnityEngine;

public class HealthPack : Interactable
{
    private PlayerHealth playerHealth;
    private bool isPickedUp = false;

    protected override void Interact()
    {
        if (!isPickedUp)
        { 
            if (playerHealth == null)
            {
                playerHealth = FindObjectOfType<PlayerHealth>();
            }

            if (playerHealth.CurrentHealth == playerHealth.MaxHealth)
            {
                return;
            }

            isPickedUp = true;
            playerHealth.Heal();
            Destroy(gameObject);
        }
    }
}
