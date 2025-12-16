using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBarrier : MonoBehaviour
{
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("PlayerObject");
    }

    // Update is called once per frame
    void Update()
    {
        if (player.transform.position.y < -2)
        {
            player.transform.position = new Vector3(164, 4, 126);
        }
    }
}
