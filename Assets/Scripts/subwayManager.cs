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
    [SerializeField] npcManager npcManagerScript;

    //station
    [SerializeField] stationManager stationScript;

    //Camera
    [SerializeField] Camera carCamera, stationCamera;

    //Tutorial
    [HideInInspector] public opponentStats tutorialStats;

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
        switchToStation();
        startCombat(tutorialStats);
    }

    public void startCombat(opponentStats opponent)
    {
        state = "combat";

        movementScript.stopWalking();
        movementScript.enabled = false;
        targetScript.enabled = false;

        //combatManagerObject.SetActive(true);
        //combatCanavas.SetActive(true);
        combatManager.instance.opponent = opponent;
        //combatManager.instance.startCombat();

        subwayUI.instance.switchHealth(true);
        StartCoroutine(TransitionManager.GetInstance().Swipe(openCombat));

    }

    public void openCombat()
    {

        combatManagerObject.SetActive(true);
        combatCanavas.SetActive(true);
        combatManager.instance.startCombat();

    }

    public void switchToMovement()
    {
        state = "movement";

        movementScript.enabled = true;
        targetScript.enabled = true;

        combatManagerObject.SetActive(false);
        combatCanavas.SetActive(false);

        subwayUI.instance.switchHealth(false);
    }

    public void switchToStation()
    {
        state = "station";

        movementScript.enabled = true;
        targetScript.enabled = true;

        combatManagerObject.SetActive(false);
        combatCanavas.SetActive(false);

        stationScript.startStation();

        subwayUI.instance.switchHealth(false);
        switchCamera("station");
    }

    public void switchToCar()
    {
        subwayUI.instance.closeUI();
        subwaySFX.Invoke();

        npcManagerScript.updateCar();   

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
