using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stationManager : MonoBehaviour
{
    [SerializeField] Transform playerSpawnPosition;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void startStation()
    {
        GameObject player = subwayManager.instance.player;
        player.GetComponent<CharacterController>().enabled = false;
        player.transform.position = playerSpawnPosition.position;
        player.transform.rotation = playerSpawnPosition.rotation;
        player.GetComponent<CharacterController>().enabled = true;
    }
}
