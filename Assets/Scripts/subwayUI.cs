using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class subwayUI : MonoBehaviour
{

    public static subwayUI instance;
    [SerializeField] nodeManager nodeManagerScript;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            nodeManager.instance = nodeManagerScript;

            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }
    }
}
