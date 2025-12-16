using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Menu
{
    public class LoadingManager : MonoBehaviour
    {
        [FormerlySerializedAs("LoadingScreen")]
        public GameObject loadingScreen;
    
        public void LoadScene(string sceneName)
        {
            StartCoroutine(LoadSceneAsync(sceneName));
        }

        IEnumerator LoadSceneAsync(string sceneName)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

            loadingScreen.SetActive(true);
        

            while (asyncLoad != null && !asyncLoad.isDone)
            {
                yield return null;
            }
        }
    }
}
