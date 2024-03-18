using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class opponentRandomizer : MonoBehaviour
{
    
    public int opponentMaxHealth { get; private set; }
    public int opponentSpeed { get; private set; }
    public int opponentAttack { get; private set; }

    [Header("Settings")]
    [SerializeField] int totalStatPoints;   
    [SerializeField] int minHealth, maxHealth;
    [SerializeField] int minSpeed, maxSpeed;
    [SerializeField] int minAttack, maxAttack;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void generateStats()
    {
        //add use to stat points (for balance)
        //stat points increases as game goes on (increases difficulty)
        //

        opponentMaxHealth = Random.Range(minHealth, maxHealth);
        opponentSpeed = Random.Range(minSpeed, maxSpeed);
        opponentAttack = Random.Range(minAttack, maxAttack);


    }
}
