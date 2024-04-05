using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npcManager : MonoBehaviour
{
    public List<GameObject> npcList; //hide in insecter after adding npc generator script and change to private set
    private opponentRandomizer opponentGenerator;

    private void Awake()
    {
        opponentGenerator = GetComponent<opponentRandomizer>();
    }

    private void Start()
    {
        addOpponents();
    }
    public void removeFromList(GameObject npc)
    {
        npcList.Remove(npc);
    }

    public void removeAll()
    {
        npcList.Clear();
    }

    public void addOpponents()
    {
        foreach (GameObject npc in npcList)
        {
            npc.GetComponent<opponentInfo>().stats = opponentGenerator.generateStats(1);
        }
    }
}
