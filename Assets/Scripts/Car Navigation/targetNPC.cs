using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class targetNPC : MonoBehaviour
{
    // used on player gameobject
    //
    //

    //to hide in inspector after making npc generator script
    
    [Header("Setup")]
    [SerializeField] GameObject targetDisplay;
    [SerializeField] npcManager npcManagerScript;

    [Header("Settings")]
    [SerializeField] float targetDistance;
    [SerializeField] float displayHeight;

    private GameObject target;
    private InputAction interact;

    void OnEnable()
    {
        interact = subwayManager.instance.playerControls.Player.Interact;
        interact.Enable();
    }

    void OnDisable()
    {
        interact.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        findClosestInteractable();
        selectTarget();
    }

    private void findClosestInteractable()
    {
        List<GameObject> npcList = npcManagerScript.talkableNPC;
        List<GameObject> doorList = subwayManager.instance.doorList;
        GameObject closestInteractable = null;

        float closestDistance = 999999;

        for (int i = 0; i < npcList.Count; i++)
        {
            float distance = Vector3.Distance(gameObject.transform.position, npcList[i].transform.position);

            if (distance < closestDistance )
            {
                closestDistance = distance;
                closestInteractable = npcList[i];
            }
        }
        
        for (int i = 0; i < doorList.Count; i++)
        {
            float distance = Vector3.Distance(gameObject.transform.position, doorList[i].transform.position);

            if (distance <closestDistance)
            {
                closestDistance = distance;
                closestInteractable = doorList[i];
            }
        }
        
        if (closestDistance < targetDistance)
        {
            targetDisplay.SetActive(true);
            moveTargetDisplay(closestInteractable.transform);
            target = closestInteractable;
        } else
        {
            targetDisplay.SetActive(false);
            target = null;
        }
    }

    private void moveTargetDisplay(Transform target)
    {
        RectTransform displayTransform = targetDisplay.GetComponent<RectTransform>();

        if (target.tag == "NPC")
        {
            Vector3 headPosition = target.Find("Head").position;
            displayTransform.anchoredPosition3D = new Vector3(headPosition.x, headPosition.y + displayHeight, headPosition.z);
        }

        displayTransform.LookAt(Camera.main.transform, Vector3.up);
        //displayTransform.anchoredPosition3D = Vector3.Lerp(displayTransform.anchoredPosition3D, Camera.main.transform.position, 0.9f);
    }

    private void selectTarget()
    {
        if (interact.ReadValue<float>() == 0) return;
        if (target == null) return;
        
        if (target.tag == "ExitDoor")
        {
            subwayManager.instance.switchToStation();
        } else if (target.tag == "NPC")
        {
            subwayManager.instance.startCombat(target.GetComponent<opponentInfo>().stats);

            npcManagerScript.removeFromList(target);
        }
    }
}
