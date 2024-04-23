using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class targetNPC : MonoBehaviour
{
    // used on player gameobject
    //
    //

    //to hide in inspector after making npc generator script

    [HideInInspector] public bool canLeaveCar;

    [Header("Setup")]
    [SerializeField] GameObject targetDisplay;
    [SerializeField] npcManager npcManagerScript;
    [SerializeField] Sprite levelOneImage, levelTwoImage, levelThreeImage, doorImage, healImage, stairImage;
    [SerializeField] GameObject statue, stairs;
    [SerializeField] stationManager stationScript;
    [SerializeField] Image playerHealthBar, tempPlayerHealthBar;

    [Header("Settings")]
    [SerializeField] float targetDistance;
    [SerializeField] float npcHeight, doorHeight, statueHeight, stairsHeight;

    private GameObject target;
    private InputAction interact;
    private Image imageComponent;
    [HideInInspector] public bool hasUsedHeal = false;

    private void Awake()
    {
        imageComponent = targetDisplay.GetComponent<Image>();
    }
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

        if (TransitionManager.GetInstance().playingTransition == false)
        {
            selectTarget();
        }
        
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

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestInteractable = npcList[i];
            }
        }

        for (int i = 0; i < doorList.Count; i++)
        {
            float distance = Vector3.Distance(gameObject.transform.position, doorList[i].transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestInteractable = doorList[i];
            }
        }

        float statueDistance = Vector3.Distance(gameObject.transform.position, statue.transform.position);

        if (statueDistance < closestDistance)
        {
            closestDistance = statueDistance;
            closestInteractable = statue;
        }

        float stairDistance = Vector3.Distance(gameObject.transform.position, stairs.transform.position);

        if (stairDistance < closestDistance)
        {
            closestDistance = stairDistance;
            closestInteractable = stairs;
        }

        if (closestDistance < targetDistance)
        {
            targetDisplay.SetActive(true);
            moveTargetDisplay(closestInteractable.transform);
            target = closestInteractable;

            switch (closestInteractable.tag)
            {
                case "NPC":
                    int npcDifficulty = Mathf.RoundToInt(closestInteractable.GetComponent<opponentInfo>().stats.difficulty);

                    switch (npcDifficulty)
                    {
                        case 0:
                            imageComponent.sprite = levelOneImage;
                            break;
                        case 1:
                            imageComponent.sprite = levelTwoImage;
                            break;
                        case 2:
                            imageComponent.sprite = levelThreeImage;
                            break;
                    }
                    break;

                case "ExitDoor":
                    if (!canLeaveCar)
                    {
                        targetDisplay.SetActive(false);
                        target = null;
                    } else
                    {
                        imageComponent.sprite = doorImage;
                    }
                    
                    break;

                case "Statue":
                    if (hasUsedHeal || gameManager.instance.playerHealth == gameManager.instance.playerMaxHealth)
                    {
                        targetDisplay.SetActive(false);
                        target = null;
                    } else
                    {
                        imageComponent.sprite = healImage;
                    }
                    break;

            }
        }
        else
        {
            targetDisplay.SetActive(false);
            target = null;
        }
    }

    private void moveTargetDisplay(Transform target)
    {
        RectTransform displayTransform = targetDisplay.GetComponent<RectTransform>();
        Vector2 viewportPoint = Vector2.zero;
        switch (target.tag)
        {
            case "NPC":
                Vector3 headPosition = target.Find("Head").position;
                viewportPoint = Camera.main.WorldToViewportPoint(new Vector3(headPosition.x, headPosition.y + npcHeight, headPosition.z));
                break;

            case "ExitDoor":
                viewportPoint = Camera.main.WorldToViewportPoint(new Vector3(target.position.x, target.position.y + doorHeight, target.position.z));
                break;

            case "Statue":
                viewportPoint = Camera.main.WorldToViewportPoint(new Vector3(target.position.x, target.position.y + statueHeight, target.position.z));
                break;

            case "Stairs":
                viewportPoint = Camera.main.WorldToViewportPoint(new Vector3(target.position.x, target.position.y + stairsHeight, target.position.z));
                break;
        }


        displayTransform.anchoredPosition = new Vector2(Screen.width * viewportPoint.x, Screen.height * viewportPoint.y);
    }

    private void selectTarget()
    {
        if (interact.ReadValue<float>() == 0) return;
        if (subwayUI.instance.state != "closed") return;

        if (target == null) return;

        if (target.tag == "ExitDoor")
        {
            StartCoroutine(TransitionManager.GetInstance().Swipe(subwayManager.instance.switchToStation));
            canLeaveCar = false;
        }
        else if (target.tag == "NPC")
        {
            subwayManager.instance.startCombat(target.GetComponent<opponentInfo>().stats);

            npcManagerScript.removeFromList(target);
            hasUsedHeal = false;
            canLeaveCar = true;
        } else if (target.tag == "Statue")
        {
            if (hasUsedHeal) return;

            gameManager.instance.playerHealth = gameManager.instance.playerMaxHealth;
            StartCoroutine(healHealth(playerHealthBar,0.3f));
            StartCoroutine(healHealth(tempPlayerHealthBar, 0.3f));
            hasUsedHeal = true;
        } else if (target.tag == "Stairs")
        {
            Debug.Log("stairs");
            stationScript.generateMenu();
        }
    }


    IEnumerator healHealth(Image bar, float duration)
    {
        float timer = 0;
        float startFillAmount = bar.fillAmount;

        while (true)
        {
            timer += Time.deltaTime;
            float lerpValue = gameManager.instance.lerpCurve.Evaluate(timer / duration);

            bar.fillAmount = Mathf.Lerp(startFillAmount, 1, lerpValue);

            if (timer >= duration)
            {
                break;
            }
            yield return null;
        }
    }
}
