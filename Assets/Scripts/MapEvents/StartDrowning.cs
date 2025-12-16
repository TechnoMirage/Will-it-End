using Health;
using System.Collections;
using UnityEngine;

namespace MapEvents
{
    public class StartDrowning : MonoBehaviour
    {
        private GameObject drowningBarrier;
        private GameObject player;

        private Coroutine damageCoroutine;

        // Start is called before the first frame update
        void Start()
        {
            drowningBarrier = GameObject.Find("DrowningBarrier");
            player = GameObject.Find("PlayerObject");
        }

        // Update is called once per frame
        void Update()
        {
            if (PlayerIsWithinBarrierScale())
            {
                if (damageCoroutine == null)
                {
                    damageCoroutine = StartCoroutine(DamagePlayerCoroutine());
                }
            }
            else
            {
                if (damageCoroutine != null)
                {
                    StopCoroutine(damageCoroutine);
                    damageCoroutine = null;
                }
            }
        }

        IEnumerator DamagePlayerCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                player.GetComponent<PlayerHealth>().TakeDamage(1);
            }
        }

        private bool PlayerIsWithinBarrierScale()
        {
            Vector3 barrierScale = drowningBarrier.transform.localScale;
            Vector3 playerPosition = player.transform.position;

            float barrierX = barrierScale.x / 2;
            float barrierZ = barrierScale.z / 2;
            float barrierY = barrierScale.y / 2;

            if (playerPosition.x > drowningBarrier.transform.position.x - barrierX &&
                playerPosition.x < drowningBarrier.transform.position.x + barrierX &&
                playerPosition.z > drowningBarrier.transform.position.z - barrierZ &&
                playerPosition.z < drowningBarrier.transform.position.z + barrierZ)
            {
                if (playerPosition.y < drowningBarrier.transform.position.y + barrierY - 0.2f)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
