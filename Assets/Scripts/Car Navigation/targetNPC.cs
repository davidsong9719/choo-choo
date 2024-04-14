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
        findClosestNPC();
        selectTarget();
    }

    private void findClosestNPC()
    {
        List<GameObject> npcList = npcManagerScript.talkableNPC;
        GameObject closestNPC = null;
        float closestDistance = 999999;

        for (int i = 0; i < npcList.Count; i++)
        {
            float distance = Vector3.Distance(gameObject.transform.position, npcList[i].transform.position);

            if (distance < closestDistance )
            {
                closestDistance = distance;
                closestNPC = npcList[i];
            }
        }
        
        if (closestDistance < targetDistance)
        {
            targetDisplay.SetActive(true);
            moveTargetDisplay(closestNPC.transform);
            target = closestNPC;
        } else
        {
            targetDisplay.SetActive(false);
            target = null;
        }
    }

    private void moveTargetDisplay(Transform target)
    {
        RectTransform displayTransform = targetDisplay.GetComponent<RectTransform>();

        Vector3 headPosition = target.Find("Head").position;
        displayTransform.anchoredPosition3D = new Vector3(headPosition.x, headPosition.y + displayHeight, headPosition.z);

        displayTransform.LookAt(Camera.main.transform, Vector3.up);
        //displayTransform.anchoredPosition3D = Vector3.Lerp(displayTransform.anchoredPosition3D, Camera.main.transform.position, 0.9f);
    }

    private void selectTarget()
    {
        if (interact.ReadValue<float>() == 0) return;
        if (target == null) return;

        subwayManager.instance.startCombat(target.GetComponent<opponentInfo>().stats);

        npcManagerScript.removeFromList(target);
    }
}
