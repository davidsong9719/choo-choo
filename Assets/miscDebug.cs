using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class miscDebug : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var pillColliders = FindObjectsOfType<CapsuleCollider>();

        for (int i= 0; i < pillColliders.Length; i++)
        {
            print(pillColliders[i].gameObject.name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
