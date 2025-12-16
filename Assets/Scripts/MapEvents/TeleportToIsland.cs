using MapEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportToIsland : MonoBehaviour
{
    private MoveUfo moveUfo;
    private GameObject playerObject;
    private GameObject ending;
    private GameObject ufo;

    // Start is called before the first frame update
    void Start()
    {
        moveUfo = GameObject.Find("FlyingUFO").GetComponent<MoveUfo>();
        playerObject = GameObject.Find("PlayerObject");
        ending = GameObject.Find("Ending");
        ufo = GameObject.Find("FlyingUFO");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerObject.GetComponent<Rigidbody>().isKinematic = true;
            playerObject.transform.position = new Vector3(ending.transform.position.x, ending.transform.position.y + 2f, ending.transform.position.z);
            StartCoroutine(ChangeUfoLocation());
            playerObject.GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    private IEnumerator ChangeUfoLocation()
    {
        yield return new WaitForSeconds(1);
        moveUfo.SetIsChasing(false);
        ufo.transform.position = new Vector3(ending.transform.position.x, moveUfo.originalY, ending.transform.position.z);
    }
}
