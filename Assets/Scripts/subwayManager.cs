using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class subwayManager : MonoBehaviour
{

    public static subwayManager instance {get; private set;}

    [Header("Setup")]
    //movement
    [SerializeField] characterMovement movementScript;
    [SerializeField] targetNPC targetScript;
    [SerializeField] Transform subwaySpawnPosition;
    [SerializeField] Transform TutorialStartPos;

    //combat
    [SerializeField] GameObject combatCanavas;
    [SerializeField] GameObject combatManagerObject;
    [SerializeField] npcManager npcManagerScript;

    //station
    [SerializeField] stationManager stationScript;

    //Camera
    [SerializeField] Camera carCamera, stationCamera, startCamera;

    //Tutorial
    public opponentStats tutorialStats;

    //Start
    [SerializeField] List<Image> startHiddenImages;
    [SerializeField] List<TextMeshProUGUI> startHiddenTexts;
    [SerializeField] List<Image> startUI;
    public bool hasStarted = false;

    //SFX
    [SerializeField] AudioClip doorSFX;
    [SerializeField] AudioSource trainSource, stationSource;


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
        hasStarted = false;
    }

    void Start()
    {
        startScreen();
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void startScreen()
    {
        state = "start";
        Time.timeScale = 0f;
        startCamera.enabled = false;
        carCamera.enabled = false;
        stationCamera.enabled = true;;

        combatManagerObject.SetActive(false);
        combatCanavas.SetActive(false);

        for (int i = 0; i < startHiddenImages.Count; i++)
        {
            startHiddenImages[i].enabled = false;
        }

        for (int i = 0; i < startHiddenTexts.Count; i++)
        {
            startHiddenTexts[i].enabled = false;
        }

        for (int i = 0; i < startUI.Count; i++)
        {
            startUI[i].gameObject.SetActive(true);
        }

        DialogueManager.GetInstance().submit.Enable();
        targetScript.interact.Enable();
    }

    public void exitGame()
    {
        Application.Quit();
    }

    public void startGame()
    {
        if (hasStarted) return;
        hasStarted = true;

        StartCoroutine(TransitionManager.GetInstance().Swipe(startGameTransition));
    }

    public void startGameTransition()
    {

        Time.timeScale = 1f;
        for (int i = 0; i < startHiddenImages.Count; i++)
        {
            startHiddenImages[i].enabled = true;
        }

        for (int i = 0; i < startHiddenTexts.Count; i++)
        {
            startHiddenTexts[i].enabled = true;
        }

        for (int i = 0; i < startUI.Count; i++)
        {
            startUI[i].gameObject.SetActive(false);
        }

        switchToStation();

        GameObject player = instance.player;
        player.GetComponent<CharacterController>().enabled = false;
        player.transform.position = TutorialStartPos.position;
        player.transform.rotation = TutorialStartPos.rotation;
        player.GetComponent<CharacterController>().enabled = true;

        state = "combat";

        movementScript.stopWalking();
        movementScript.enabled = false;
        targetScript.enabled = false;

        combatManager.instance.opponent = tutorialStats;
        combatManagerObject.SetActive(true);

        openCombat();
    }

    public void startCombat(opponentStats opponent)
    {
        state = "combat";

        movementScript.stopWalking();
        movementScript.enabled = false;
        targetScript.enabled = false;

        combatManager.instance.opponent = opponent;
        combatManagerObject.SetActive(true);

        StartCoroutine(TransitionManager.GetInstance().Swipe(openCombat));
    }

    public void openCombat()
    {
        subwayUI.instance.switchHealth(true);
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

        subwayUI.instance.closeUI();
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
        startCamera.enabled = false;
        if (newView == "station")
        {
            stationCamera.enabled = true;
            carCamera.enabled = false;
        } else if (newView == "car")
        {
            carCamera.enabled = true;
            stationCamera.enabled = false;
        }

    }


    public void startPlayDoorSFX()
    {
        StartCoroutine(playDoorSFX());
    }

    IEnumerator playDoorSFX()
    {
        gameManager.instance.setVolume(0.2f);
        gameManager.instance.playSFX(doorSFX);

        yield return new WaitForSecondsRealtime(0.68f);

        if (instance.state == "station")
        {
            trainSource.Pause();
            stationSource.Play();
        } else
        {
            trainSource.Play();
            stationSource.Pause();
        } 
    }

    public IEnumerator fadeVolume()
    {
        float volumeFadeTime = 4;
        float stationStartVolume = stationSource.volume;
        float timeCounter = -Time.unscaledDeltaTime;
        
        while(timeCounter < volumeFadeTime)
        {
            timeCounter += Time.unscaledDeltaTime;
            stationSource.volume = Mathf.Lerp(stationStartVolume, 0f, timeCounter/volumeFadeTime);
            yield return null;
        }
    }
}
