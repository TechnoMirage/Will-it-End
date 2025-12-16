using Gun.Gun;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

namespace Menu
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private GunScriptableObject uziGun;
        [SerializeField] private GunScriptableObject m4A1Gun;
        [FormerlySerializedAs("StartButton")] [SerializeField] private GameObject _StartButton;
        private GameObject _chooseWeapon;
        private GameObject _loadingScreen;
        [SerializeField]
        private GameObject _mainMenuTitle;
        [SerializeField]
        private GameObject _mainMenuStartButton;
        [SerializeField]
        private GameObject _mainMenu;
        private LoadingManager _loadingManager;
        [SerializeField]
        private GameObject _spinner;
        [SerializeField]
        private GameObject _gunBtnM4A1;

        private GameObject _background;
        private void Start()
        {
            _mainMenu = GameObject.FindWithTag("mainMenu");
            _chooseWeapon = GameObject.FindWithTag("weaponMenu");
            _loadingScreen = GameObject.FindWithTag("loadingScreen");
            _loadingManager = FindObjectOfType<LoadingManager>();
            _background = GameObject.Find("Background");
            _mainMenu.SetActive(true);
            _gunBtnM4A1 = GameObject.Find("gunBtnM4A1");
            _chooseWeapon.SetActive(false);
            _loadingScreen.SetActive(false);
            _background.SetActive(true);
            _spinner.SetActive(false);
                
        }

        public void OpenGunMenu()
        { 
            
            _background.SetActive(true);
            _chooseWeapon.SetActive(true);
            _mainMenu.SetActive(false);
            _mainMenuTitle.SetActive(false);
            _mainMenuStartButton.SetActive(false);
            EventSystem.current.SetSelectedGameObject(_gunBtnM4A1);
            var startButton = _StartButton.GetComponent<Button>();
           
            startButton.interactable = false;
        }

        public void PlayGame()
        {
            _mainMenu.SetActive(false);
            _chooseWeapon.SetActive(false);
            _loadingScreen.SetActive(true);
            _background.SetActive(false);
            _spinner.SetActive(true);
            _loadingManager.LoadScene("SampleScene");
            
           
        }


        public void OpenOptions()
        {
            Debug.Log("Options");
        }

        public void OpenMainMenu()
        {   
            EventSystem.current.SetSelectedGameObject(_mainMenuStartButton);
            _background.SetActive(true);
            _mainMenu.SetActive(true);
            _mainMenuTitle.SetActive(true);
            _mainMenuStartButton.SetActive(true);
            _chooseWeapon.SetActive(false);
        }

        public void ChooseUzi()
        {
            PlayerPrefs.SetString("SelectedGun", uziGun.name);
            Debug.Log(PlayerPrefs.GetString("SelectedGun"));
            var startButton = _StartButton.GetComponent<Button>();
            startButton.interactable = true;
        }

        public void ChooseM4A1()
        {
            PlayerPrefs.SetString("SelectedGun", m4A1Gun.name);
            Debug.Log(PlayerPrefs.GetString("SelectedGun"));
            var startButton = _StartButton.GetComponent<Button>();
            startButton.interactable = true;
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
