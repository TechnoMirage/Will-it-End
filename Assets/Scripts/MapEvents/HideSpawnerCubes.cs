using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideSpawnerCubes : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform chiled in transform)
        {
            chiled.gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
    }
}
