using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameManager : MonoBehaviour
{
    public static gameManager instance { get; private set; }

    [Header("Public Access")]
    public List<card> playerDeck;
    public int playerMaxHealth, playerHealth, playerSpeed, playerAttack, playerCash;

    public card attack, defend;

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
