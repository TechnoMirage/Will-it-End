using UnityEngine;

namespace MapEvents
{
    public class InvisibleWalls : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            //Turn walls invisible
            foreach (GameObject wall in GameObject.FindGameObjectsWithTag("InvisibleWall"))
            {
                wall.GetComponent<Renderer>().enabled = false;
            }
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
