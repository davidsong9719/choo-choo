using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class opponentInfo : MonoBehaviour
{
    public opponentStats stats;
    public Transform transformParent;

    [SerializeField] string animationName;
    // Start is called before the first frame update
    void Start()
    {
        Animation animation = GetComponentInChildren<Animation>();

        switch(animationName)
        {
            case "npcIdle1":
            case "npcIdle2":
            case "npcIdle3":
                Animator anim = GetComponentInChildren<Animator>();
                anim.Play(animationName, 0, Random.Range(0f, 1f));
                anim.speed = Random.Range(0.9f, 1.1f);
                break;

            default:
                Debug.Log("invalid npc animation detected");
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
