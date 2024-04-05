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

    [Header("Public Access")]
    public GameObject player;
    public string state;
    public string previousState; //for opening
    public PlayerInputActions playerControls;

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

        combatManagerObject.SetActive(true);
        combatCanavas.SetActive(true);
    }

    public void startCombat(opponentStats opponent)
    {
        state = "combat";

        movementScript.enabled = false;
        targetScript.enabled = false;

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

        combatManagerObject.SetActive(false);
        combatCanavas.SetActive(false);
    }
}
