using UnityEngine;

namespace MapEvents
{
    public class SpinDummy : MonoBehaviour
    {
        public GameObject dummy;

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            dummy.transform.Rotate(Random.Range(0, 360) * Time.deltaTime, Random.Range(0, 360) * Time.deltaTime, Random.Range(0, 360) * Time.deltaTime);
        }
    }
}
