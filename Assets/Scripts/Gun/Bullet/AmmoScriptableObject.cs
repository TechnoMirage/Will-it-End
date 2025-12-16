using System.Collections;
using Inventory;
using UnityEngine;

namespace Gun.Bullet
{
    [CreateAssetMenu(fileName = "Ammo Config", menuName = "Guns/Ammo Config", order = 3)]
    public class AmmoScriptableObject : ScriptableObject
    {
        public int DefaultMaxAmmo = 120;
        public int DefaultClipSize = 30;

        public int MaxAmmo = 120;
        public int ClipSize = 30;
        public int CurrentAmmo = 120;
        public int CurrentClipAmmo = 30;

    
        public void Reload()
        {
            InventoryManager inventoryManager = FindAnyObjectByType<InventoryManager>();

            int maxRealoadAmount = Mathf.Min(ClipSize, CurrentAmmo);
            int availableAmmoInClip = ClipSize - CurrentClipAmmo;
            int reloadAmount = Mathf.Min(maxRealoadAmount, availableAmmoInClip);
        
            CurrentClipAmmo = CurrentClipAmmo + reloadAmount;
            CurrentAmmo -= reloadAmount;

            inventoryManager.RefreshGuns();

        }
        
        public bool CanReload()
        {
            return CurrentClipAmmo < ClipSize && CurrentAmmo > 0;
        }
    }
}