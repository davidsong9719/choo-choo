using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameManager : MonoBehaviour
{
    public static gameManager instance { get; private set; }

    [Header("Public Access")]
    public List<card> playerDeck;
    public int playerMaxHealth, playerHealth, playerSpeed, playerAttack, playerCash;
    public int followerAmount;
    public int timeElapsed;

    [Header("Templates")]
    public card attack, defend;
    public GameObject attackCardPrefab, defendCardPrefab, effectCardPrefab, nullCardPrefab;

    [Header("Parameters")]
    public int stageOneLength;
    public int stageTwoLength;
    public int stageThreeLength;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            cardGenerator.instance = GetComponent<cardGenerator>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

}
