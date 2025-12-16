using Gun.Gun;
using Player;
using Scripts.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel;    
    [SerializeField] private GameObject weaponPanel;

    [SerializeField] private Transform gunSlots;
    [SerializeField] private Transform grenadeSlots;
    [SerializeField] private Transform ammoSlots;
    [SerializeField] private Transform inventorySlots;

    private int maxGunNumber = 2;
    [SerializeField] private Sprite gunIcon;
    
    [SerializeField] private int maxGrenadeNumber = 3;
    [SerializeField] private int totalGrenade;

    public GameObject grenadePrefab;
    public GameObject playerLocation;
    private bool isInventoryOpen = false;

    PlayerGunSelector playerGunSelector;
    GunScriptableObject primaryGun;
    GunScriptableObject secondaryGun;
    PlayerInputScript playerInput;
    
    void Awake()
    {
        playerInput = new PlayerInputScript();
        playerInput.Enable();
    }
    void Start()
    {
        
        playerGunSelector = FindObjectOfType<PlayerGunSelector>();
        if (playerGunSelector == null)
        {
            Debug.LogError("PlayerGunSelector not found.");
        }
        if (playerGunSelector.Guns.Count < 2)
        {
            Debug.LogError("Insufficient number of guns in the playerGunSelector.Guns list.");
            return; 
        }
        primaryGun = playerGunSelector.Guns[0];
        secondaryGun = playerGunSelector.Guns[1];
        RefreshGuns();
        RefreshGrenades();
        RefreshTotalAmmo();
    }

    void Update()
    {
        if (playerInput.FPSController.Inventory.triggered)
        {
            isInventoryOpen = !isInventoryOpen;
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
            weaponPanel.SetActive(!weaponPanel.activeSelf);
        }
        
        if (Input.GetKeyDown(KeyCode.O))
        {
            if (isInventoryOpen is false)
            {
                ChangeGun();
            }
        }
    }

    public void ChangeGun()
    {
        GunScriptableObject gun;

        gun = primaryGun;
        primaryGun = secondaryGun;
        secondaryGun = gun;

        RefreshGuns();
    }

    public void RefreshGuns()
    {
        RefreshPrimaryGun();
        RefreshSecondaryGun();
        RefreshTotalAmmo();
    }

    private void RefreshPrimaryGun()
    {
        if (primaryGun != null)
        {
            gunSlots.GetChild(0).GetComponent<Image>().sprite= primaryGun.GunIcon;
            ammoSlots.GetChild(0).GetComponent<TextMeshProUGUI>().text = "x " + primaryGun.AmmoConfig.CurrentClipAmmo + "/" + primaryGun.AmmoConfig.ClipSize;
            inventorySlots.GetChild(0).GetChild(0).GetComponent<Image>().sprite = primaryGun.GunIcon;
            inventorySlots.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = primaryGun.AmmoConfig.CurrentClipAmmo + "/" + primaryGun.AmmoConfig.ClipSize;
        }
        else
        {
            gunSlots.GetChild(0).GetComponent<Image>().sprite = gunIcon;
            ammoSlots.GetChild(0).GetComponent<TextMeshProUGUI>().text = "x 0/0";
            inventorySlots.GetChild(0).GetChild(0).GetComponent<Image>().sprite = gunIcon;
            inventorySlots.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "x 0/0";
        }
    }

    private void RefreshSecondaryGun()
    {
        if (secondaryGun != null)
        {
            gunSlots.GetChild(1).GetComponent<Image>().sprite = secondaryGun.GunIcon;
            ammoSlots.GetChild(1).GetComponent<TextMeshProUGUI>().text = "x " + secondaryGun.AmmoConfig.CurrentClipAmmo + "/" + secondaryGun.AmmoConfig.ClipSize;
            inventorySlots.GetChild(0).GetChild(1).GetComponent<Image>().sprite = secondaryGun.GunIcon;
            inventorySlots.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = secondaryGun.AmmoConfig.CurrentClipAmmo + "/" + secondaryGun.AmmoConfig.ClipSize;
        }
        else
        {
            gunSlots.GetChild(1).GetComponent<Image>().sprite = gunIcon; 
            ammoSlots.GetChild(1).GetComponent<TextMeshProUGUI>().text = "x 0/0";
            inventorySlots.GetChild(0).GetChild(1).GetComponent<Image>().sprite = gunIcon;
            inventorySlots.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = "x 0/0";
        }
    }

    private void RefreshGrenades()
    {
        grenadeSlots.GetChild(1).GetComponent<TextMeshProUGUI>().text = "x " + totalGrenade.ToString();
        inventorySlots.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = "x " + totalGrenade.ToString();
    }

    public bool RemoveGrenade(int nbGrenade)
    {
        if ((totalGrenade - nbGrenade) >= 0)
        {
            totalGrenade -= nbGrenade;
            RefreshGrenades();
            return true;
        }

        return false;
    }

    public bool AddGrenade(int nbGrenade)
    {
        if ((totalGrenade + nbGrenade) <= maxGrenadeNumber)
        {
            totalGrenade += nbGrenade;
            RefreshGrenades();
            return true;
        }

        return false;
    }

    private bool RemoveAmmo(int nbAmmo)
    {
        if ((primaryGun.AmmoConfig.CurrentAmmo - nbAmmo) >= 0)
        {
            primaryGun.AmmoConfig.CurrentAmmo -= nbAmmo;
            RefreshTotalAmmo();
            return true;
        }

        return false;
    }

    private int ComplexeRemoveAmmo()
    {
        return primaryGun.AmmoConfig.CurrentAmmo;
    }

    private bool AddAmmo(int nbAmmo)
    {
        if ((primaryGun.AmmoConfig.CurrentAmmo + nbAmmo) <= primaryGun.AmmoConfig.MaxAmmo)
        {
            primaryGun.AmmoConfig.CurrentAmmo += nbAmmo;
            RefreshTotalAmmo();
            return true;
        }

        return false;
    }

    private int ComplexeAddAmmo()
    {
        return primaryGun.AmmoConfig.MaxAmmo - primaryGun.AmmoConfig.CurrentAmmo;
    }

    private void RefreshTotalAmmo()
    {
        if (primaryGun == null)
        {
            return;
        }
        ammoSlots.GetChild(2).GetComponent<TextMeshProUGUI>().text = "x " + primaryGun.AmmoConfig.CurrentAmmo.ToString();
        inventorySlots.GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>().text = "x " + primaryGun.AmmoConfig.CurrentAmmo.ToString();
    }
}