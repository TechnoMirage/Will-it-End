using Gun.Gun;
using Player;
using UnityEngine;
namespace Scripts.Player
{
    public class PlayerAction : MonoBehaviour
    {
        [SerializeField]
        private PlayerGunSelector GunSelector;
       
        private bool _IsSwitchingWeapon;

        public GameObject grenadePrefab;
        public GameObject playerLocation;
        public GameObject cameraOrientation;
   
        [SerializeField] 
        private float ReloadSpeed = 1f;
        InventoryManager inventoryManager;
        private PlayerInputScript playerInput;
        private void Start()
        {
            GunSelector = FindObjectOfType<PlayerGunSelector>();

            if (GunSelector == null)
            {
                return; 
            }
            var gunpref = PlayerPrefs.GetString("SelectedGun");
            var selectedGun = GunSelector.gunDatabase.guns.Find(gun => gun.Name == gunpref);

            if (selectedGun != null)
            {
                GunSelector.Guns.Add(selectedGun);

            }
            
            inventoryManager = FindObjectOfType<InventoryManager>();
        }
        private void Awake()
        {
            playerInput = new PlayerInputScript();
            playerInput.Enable();
        }
       
        private void Update()
        {
            if (playerInput.FPSController.Switch.triggered)
            {
                StartSwitchWeapon();
                GunSelector.SwitchGun();
                EndSwitchWeapon();
                
            }
            
            if (playerInput.FPSController.Throw.triggered)
            {
                throwGrenade();
            }

            if (GunSelector.ActiveGun != null && !_IsSwitchingWeapon)
            {
                if (GunSelector.ActiveGun.IsReloading)
                {
                    return;
                }
                GunSelector.ActiveGun.Tick(playerInput.FPSController.Shoot.IsPressed());

            }

            if (ShouldManualReload())
            {
                GunSelector.ActiveGun.StartReload();
            }
        }

        void throwGrenade()
        {
            if (inventoryManager.RemoveGrenade(1) == true)
            {
                GameObject grenade = Instantiate(grenadePrefab, playerLocation.transform.position + cameraOrientation.transform.forward, Quaternion.identity);
                grenade.SendMessage("ChangeExplosionType");
                grenade.GetComponent<Rigidbody>().AddForce(cameraOrientation.transform.forward * 10, ForceMode.Impulse);
            }
        }

        private bool ShouldManualReload()
        {
            if (playerInput.FPSController.Reload.IsPressed() && GunSelector.ActiveGun.CanReload() && !GunSelector.ActiveGun.IsReloading)
            {
                return true;
            }
            return false;
        }
        
        public void StartSwitchWeapon()
        {

            _IsSwitchingWeapon = true;
        }
        
        public void EndSwitchWeapon()
        {
            _IsSwitchingWeapon = false;
        }
    }
}
