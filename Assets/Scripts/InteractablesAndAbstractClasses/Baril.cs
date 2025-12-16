using System.Collections;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;

public class Baril : Interactable
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void Interact()
    {
        print("video a 10 minutes" + gameObject.name);
    }
}
