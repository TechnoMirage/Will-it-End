using Steamworks;
using System.Collections;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

namespace MapEvents
{
    public class MoveUfo : MonoBehaviour
    {
        public GameObject ufo;
        public int madness = 0;
        public AudioClip angryAliens;

        public float originalY;
        private bool isChasing = false;
        private AudioSource audioSource;
        private GameObject raisingCylinder;
        private float raisingCylinderRadius = 7f;
        private GameObject playerObject;

        // Start is called before the first frame update
        void Start()
        {
            originalY = ufo.transform.position.y;
            audioSource = GetComponent<AudioSource>();
            audioSource.clip = angryAliens;

            raisingCylinder = gameObject.transform.Find("RaisingCylinder").gameObject;
            raisingCylinder.GetComponent<MeshRenderer>().enabled = false;
            playerObject = GameObject.Find("PlayerObject");
        }

        // Update is called once per frame
        void Update()
        {
            if (isChasing)
            {
                if (IsPlayerWithinRange())
                {
                    AbductPlayer();
                    playerObject.GetComponent<Rigidbody>().useGravity = false;
                }
                else
                {
                    ChasePlayer();
                    playerObject.GetComponent<Rigidbody>().useGravity = true;
                }
            }
            else
            {
                playerObject.GetComponent<Rigidbody>().useGravity = true;
            }


            if (ufo != null && isChasing == false)
            {
                ufo.transform.Rotate(Vector3.up * Time.deltaTime * 50);
                ufo.transform.position = new Vector3(ufo.transform.position.x, originalY + Mathf.PingPong(Time.time * 10, 30), ufo.transform.position.z);
            }
        }

        public void IncreaseMadness()
        {
            madness++;

            if (madness == 100)
            {
                SetIsChasing(true);
                audioSource.Play();
            }
        }

        private void ChasePlayer()
        {
            if (ufo != null)
            {
                Vector3 playerPosition = playerObject.transform.position;
                ufo.transform.Rotate(Vector3.up * Time.deltaTime * 50);
                ufo.transform.position = Vector3.MoveTowards(ufo.transform.position, new Vector3(playerPosition.x, playerPosition.y + 40, playerPosition.z), Time.deltaTime * 20);
            }
        }

        private void AbductPlayer()
        {
            if (ufo != null)
            {
                Vector3 playerPosition = playerObject.transform.position;

                ufo.transform.Rotate(Vector3.up * Time.deltaTime * 50);
                ufo.transform.position = Vector3.MoveTowards(ufo.transform.position, new Vector3(playerPosition.x, gameObject.transform.position.y, playerPosition.z), Time.deltaTime * 5);

                if (playerObject.transform.position.y < ufo.transform.position.y)
                {
                    playerObject.transform.position = new Vector3(playerObject.transform.position.x, playerObject.transform.position.y + 0.5f, playerObject.transform.position.z);
                }
            }
        }

        private bool IsPlayerWithinRange()
        {
            if (ufo != null)
            {
                if (playerObject != null)
                {
                    Vector3 playerPosition = playerObject.transform.position;
                    Vector3 ufoPosition = ufo.transform.position;
                    float distanceXZ = Vector3.Distance(new Vector3(playerPosition.x, 0, playerPosition.z), new Vector3(ufoPosition.x, 0, ufoPosition.z));

                    if (distanceXZ <= raisingCylinderRadius)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void SetIsChasing(bool value)
        {
            isChasing = value;
        }
    }
}