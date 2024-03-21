using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraFollow : MonoBehaviour
{
    [SerializeField] GameObject target;

    private Vector3 transformOffset;
    // Start is called before the first frame update
    void Start()
    {
        transformOffset = transform.position - target.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        follow();
    }

    private void follow()
    {
        transform.position = target.transform.position + transformOffset;
        transform.LookAt(target.transform, Vector3.up);

    }
}
