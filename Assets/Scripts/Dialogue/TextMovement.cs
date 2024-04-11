using System.Collections;
using TMPro;
using UnityEngine;

public class TextMovement : MonoBehaviour
{
    [SerializeField] Transform[] positions;
    public int speed;

    [SerializeField] GameObject[] textBoxes;

    private Vector3 targetPos;

    private static TextMovement instance;

    public bool inPlace = true;


    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Found more than one Dialogue Manager in scene");
        }
        instance = this;
        inPlace = true;

    }
    public static TextMovement GetInstance()
    {
        return instance;
    }

    public void moveBoxes()
    {
        {
            //if (inPlace) {
                for (int i = 0; i < textBoxes.Length; i++)
                {
                    Debug.Log("moving " + textBoxes[i].name);
                    if (Vector2.Distance(textBoxes[i].transform.position, positions[3].position) < .1f)
                    {
                        //swap target position
                        targetPos = positions[2].position;
                        inPlace = true;
                    }
                    //continue for all other positions
                    else if (Vector2.Distance(textBoxes[i].transform.position, positions[2].position) < .1f)
                    {
                        //swap target position
                        targetPos = positions[1].position;
                        inPlace = true;
                    }
                    else if (Vector2.Distance(textBoxes[i].transform.position, positions[1].position) < .1f)
                    {
                        //swap target position
                        targetPos = positions[0].position;
                        inPlace = true;
                    }
                    else if (Vector2.Distance(textBoxes[i].transform.position, positions[0].position) < .1f)
                    {
                        //swap target position
                        textBoxes[i].transform.position = positions[3].position;
                        textBoxes[i].GetComponentInChildren<TextMeshProUGUI>().text = "";
                        targetPos = positions[3].position;
                        inPlace = true;
                    }
                    else
                    {
                        Debug.LogWarning("Cant find where to go");
                        inPlace = false;
                    }
                    //if (inPlace)
                    //{
                        StartCoroutine(moveUp(textBoxes[i], targetPos));
                    //}
                //}
            }
        }  
    }

    private IEnumerator moveUp(GameObject textbox, Vector3 target)
    {
        while (textbox.transform.position != target)
            {
                inPlace = false;
                textbox.transform.position = Vector3.MoveTowards(textbox.transform.position, target, speed * Time.deltaTime);
                yield return null;
            }
        
        inPlace = true;
    }

    public void ResetPos()
    {
        for(int i = 0; i < textBoxes.Length; i++)
        {
            textBoxes[i].transform.position = positions[i].position;
        }
    }

}
