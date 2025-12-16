using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinGame : MonoBehaviour
{
    private GameObject playerObject;
    private bool isPlayerWithinRange = false;
    private SceneManager sceneManager;

    // Start is called before the first frame update
    void Start()
    {
        playerObject = GameObject.Find("PlayerObject");
    }

    // Update is called once per frame
    void Update()
    {
        if (IsPlayerWithinRange())
        {
            if (!isPlayerWithinRange)
            {
                isPlayerWithinRange = true;
                SceneManager.LoadScene("VictoryMenu");
            }
        }
    }

    private bool IsPlayerWithinRange()
    {
        return Vector3.Distance(playerObject.transform.position, transform.position) < 2f;
    }
}
