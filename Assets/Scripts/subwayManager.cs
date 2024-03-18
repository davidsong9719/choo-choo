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
    [SerializeField] GameObject combatManager;

    [Header("Public Access")]
    public GameObject player;
    public string state;
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

        combatManager.SetActive(true);
        combatCanavas.SetActive(true);
    }

    public void switchToMovement()
    {
        state = "movement";

        movementScript.enabled = true;
        targetScript.enabled = true;

        combatManager.SetActive(false);
        combatCanavas.SetActive(false);
    }

}
