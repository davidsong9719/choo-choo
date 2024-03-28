using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class mapButton : MonoBehaviour
{
    [SerializeField] GameObject mapManager;
    
    // Start is called before the first frame update

    private void Awake()
    {
        mapManager.SetActive(true);
    }

    public void toggleMapVisibility(bool mapVisible)
    {
        for (int i = 0; i < mapManager.transform.childCount; i++)
        {
            for (int j = 0; j < mapManager.transform.GetChild(i).childCount; j++)
            {
                for (int k = 0; k < mapManager.transform.GetChild(i).GetChild(j).childCount; k++)
                {
                    Image imageComponentK = mapManager.transform.GetChild(i).GetChild(j).GetChild(k).GetComponent<Image>();
                    if (imageComponentK == null) continue;

                    imageComponentK.enabled = mapVisible;
                }
                Image imageComponentJ = mapManager.transform.GetChild(i).GetChild(j).GetComponent<Image>();
                if (imageComponentJ == null) continue;

                imageComponentJ.enabled = mapVisible;
            }

            Image imageComponentI = mapManager.transform.GetChild(i).GetComponent<Image>();
            if (imageComponentI == null) continue;

            imageComponentI.enabled = mapVisible;
        }
    }

}
