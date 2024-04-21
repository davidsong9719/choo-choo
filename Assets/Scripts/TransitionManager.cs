using Ink.UnityIntegration;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TransitionManager : MonoBehaviour
{
    public Animator swipe;


    private static TransitionManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Found more than one Dialogue Manager in scene");
        }
        instance = this;
    }
    public static TransitionManager GetInstance()
    {
        return instance;
    }

    public IEnumerator Swipe(Action function)
    {
        swipe.SetTrigger("end");
        yield return new WaitForSeconds(0.7f);
        function();
        swipe.SetTrigger("start");
    }

    /*public IEnumerator Doors(Action function)
    {
        swipe.SetTrigger("end");
        yield return new WaitForSeconds(1.5f);
        function();
        swipe.SetTrigger("start");
    }*/


}
