using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class subwayManager : MonoBehaviour
{

    public static subwayManager instance {get; private set;}

    [Header("Setup")]
    //movement
    [SerializeField] characterMovement movementScript;
    [SerializeField] targetNPC targetScript;
    [SerializeField] Transform subwaySpawnPosition;

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
    public UnityEvent subwaySFX;

    private void Awake()
    {
        playerControls = new PlayerInputActions();
        instance = this;

    }

    void Start()
    {
        switchToCar();
    }


    public void switchToCombat()
    {
        state = "combat";

        movementScript.enabled = false;
        targetScript.enabled = false;
        //stationScript.enabled = false;

        combatManagerObject.SetActive(true);
        combatCanavas.SetActive(true);

    }

    public void startCombat(opponentStats opponent)
    {
        state = "combat";

        movementScript.stopWalking();
        movementScript.enabled = false;
        targetScript.enabled = false;
       // stationScript.enabled = false;

        combatManagerObject.SetActive(true);
        combatCanavas.SetActive(true);
        combatManager.instance.opponent = opponent;
        combatManager.instance.startCombat();

    }

    public void switchToMovement()
    {
        state = "movement";

        movementScript.enabled = true;
        targetScript.enabled = true;
        //stationScript.enabled = false;

        combatManagerObject.SetActive(false);
        combatCanavas.SetActive(false);

    }

    public void switchToStation()
    {
        state = "station";

        movementScript.enabled = true;
        targetScript.enabled = true;
        //stationScript.enabled = true;

        stationScript.startStation();

        switchCamera("station");
    }

    public void switchToCar()
    {
        subwaySFX.Invoke();

        switchToMovement();
        switchCamera("car");

        GameObject player = instance.player;
        player.GetComponent<CharacterController>().enabled = false;
        player.transform.position = subwaySpawnPosition.position;
        player.transform.rotation = subwaySpawnPosition.rotation;
        player.GetComponent<CharacterController>().enabled = true;
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
