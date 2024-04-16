using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class subwayManager : MonoBehaviour
{

    public static subwayManager instance {get; private set;}

    [Header("Setup")]
    //movement
    [SerializeField] characterMovement movementScript;
    [SerializeField] targetNPC targetScript;

    //combat
    [SerializeField] GameObject combatCanavas;
    [SerializeField] GameObject combatManagerObject;

    //station
    [SerializeField] stationManager stationScript;

    //Camera
    [SerializeField] Camera carCamera, stationCamera;

    [Header("Public Access")]
    public GameObject player;
    public string state;
    public string previousState; //for opening
    public PlayerInputActions playerControls;
    public List<GameObject> doorList;

    private void Awake()
    {
        playerControls = new PlayerInputActions();
        instance = this;

    }

    void Start()
    {
        switchToMovement();
    }


    public void switchToCombat()
    {
        state = "combat";

        movementScript.enabled = false;
        targetScript.enabled = false;
        stationScript.enabled = false;

        combatManagerObject.SetActive(true);
        combatCanavas.SetActive(true);

        switchCamera("car");
    }

    public void startCombat(opponentStats opponent)
    {
        state = "combat";

        movementScript.stopWalking();
        movementScript.enabled = false;
        targetScript.enabled = false;
        stationScript.enabled = false;

        combatManagerObject.SetActive(true);
        combatCanavas.SetActive(true);
        combatManager.instance.opponent = opponent;
        combatManager.instance.startCombat();

        switchCamera("car");
    }

    public void switchToMovement()
    {
        state = "movement";

        movementScript.enabled = true;
        targetScript.enabled = true;
        stationScript.enabled = false;

        combatManagerObject.SetActive(false);
        combatCanavas.SetActive(false);

        switchCamera("car");
    }

    public void switchToStation()
    {
        state = "station";

        movementScript.enabled = true;
        targetScript.enabled = false;
        stationScript.enabled = true;

        stationScript.startStation();

        switchCamera("station");
    }

    private void switchCamera(string newView)
    {
        if (newView == "station")
        {
            stationCamera.enabled = true;
            carCamera.enabled = false;
        } else if (newView == "car")
        {
            stationCamera.enabled = false;
            carCamera.enabled = true;
        }
    }
}
