using System;
using System.Collections;
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
        GetComponent<AudioSource>().Play();
        playingTransition = true;
        swipe.SetTrigger("end");
        yield return new WaitForSecondsRealtime(0.6f);
        function();
        subwayUI.instance.setGuideTextToPerm();
        swipe.SetTrigger("start");
        yield return new WaitForSecondsRealtime(1f);
        playingTransition = false;
    }

    /*public IEnumerator Doors(Action function)
    {
        swipe.SetTrigger("end");
        yield return new WaitForSeconds(1.5f);
        function();
        swipe.SetTrigger("start");
    }*/
    public IEnumerator End()
    {
        playingTransition = true;
        swipe.SetTrigger("end");
        yield return new WaitForSecondsRealtime(0.6f);
        StartCoroutine(subwayUI.instance.rollCredits());
        //yield return new WaitForSecondsRealtime(1f);
        //playingTransition = false;
    }

}
