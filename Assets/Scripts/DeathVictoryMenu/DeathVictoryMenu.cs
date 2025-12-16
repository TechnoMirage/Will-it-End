using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menu
{
    public class DeathMenu : MonoBehaviour
    {
        public void Start()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
           
        }
        public void OpenMainMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
