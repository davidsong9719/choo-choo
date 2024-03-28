using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npcManager : MonoBehaviour
{
    public List<GameObject> npcList; //hide in insecter after adding npc generator script and change to private set
    public void removeFromList(GameObject npc)
    {
        npcList.Remove(npc);
    }

    public void removeAll()
    {
        npcList.Clear();
    }
}
