using System.Collections.Generic;
using Gun.Gun;
using Inventory;
using UnityEngine;

namespace Player
{
    [DisallowMultipleComponent]
    public class PlayerGunSelector : MonoBehaviour
    {
        [SerializeField] private GunType Gun;
        [SerializeField] private Transform GunParent;
        [SerializeField] public List<GunScriptableObject> Guns;

        [Space] [Header("Runtime Filled")] public GunScriptableObject ActiveGun;

        public GunDatabase gunDatabase;

        private int _currentGunIndex;
        
        private void Start()
        {
            ResetGunsStatsBeforeGame();

            if (Guns == null)
            {
                Guns = new List<GunScriptableObject>();
            }

            var gunpref = PlayerPrefs.GetString("SelectedGun");
            var selectedGun = gunDatabase.guns.Find(gun => gun.Name == gunpref);
            
            if (selectedGun != null)
            {
                Guns.Add(selectedGun);
            }

            if (Guns.Count > 0)
            {
                ActiveGun = Guns[0];
                ActiveGun.Spawn(GunParent, this);
            }
        }

        private void ResetGunsStatsBeforeGame()
        {
            gunDatabase.guns.ForEach(gun =>
            {
                if (gun.IsReloading)
                {
                    gun.IsReloading = false;
                }

                gun.AmmoConfig.ClipSize = gun.AmmoConfig.DefaultClipSize;
                gun.AmmoConfig.MaxAmmo = gun.AmmoConfig.DefaultMaxAmmo;

                gun.AmmoConfig.CurrentAmmo = gun.AmmoConfig.DefaultMaxAmmo;
                gun.AmmoConfig.CurrentClipAmmo = gun.AmmoConfig.DefaultClipSize;

                gun.DamageConfig.DamageRange = gun.DamageConfig.DefaultDamageRange;
            });
        }

        public void SwitchGun()
        {
            var inventoryManager = FindAnyObjectByType<InventoryManager>();

            ActiveGun.Despawn();


            _currentGunIndex++;
            

            if (_currentGunIndex == Guns.Count-1)
            {
                _currentGunIndex = 0;
            }

            ActiveGun = Guns[_currentGunIndex];

            ActiveGun.Spawn(GunParent, this);
            inventoryManager.ChangeGun();
        }

        private T FindAnyObjectByType<T>() where T : Object
        {
            var objects = FindObjectsOfType<T>();
            if (objects.Length > 0) return objects[0];

            Debug.LogWarning($"No object of type {typeof(T)} found in the scene.");
            return null;
        }
    }
}