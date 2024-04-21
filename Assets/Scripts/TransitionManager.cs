using Ink.UnityIntegration;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TransitionManager : MonoBehaviour
{
    public Animator swipe;
    [HideInInspector] public bool playingTransition;

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
        playingTransition = true;
        swipe.SetTrigger("end");
        //subwayManager.instance.playerControls.Player.Interact.Disable();
        yield return new WaitForSeconds(0.7f);
        Debug.Log("waiting");
        function();
        swipe.SetTrigger("start");
        yield return new WaitForSeconds(1f);
        //subwayManager.instance.playerControls.Player.Interact.Enable();
        playingTransition = false;
        Debug.Log("waiting again");
    }

    /*public IEnumerator Doors(Action function)
    {
        swipe.SetTrigger("end");
        yield return new WaitForSeconds(1.5f);
        function();
        swipe.SetTrigger("start");
    }*/


}
