using Ink.Parsed;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextMovement : MonoBehaviour
{
    [SerializeField] Transform[] positions;
    public int speed;

    [SerializeField] GameObject[] textBoxes;
    [SerializeField] Canvas[] sortOrder;

    private Vector3 targetPos;

    private static TextMovement instance;

    public bool inPlace = true;
    private Dictionary<GameObject, bool> inPlaceDict;


    private void Awake()
    {
        inPlaceDict = new Dictionary<GameObject, bool> { };

        for (int i = 0; i < textBoxes.Length; i++)
        {
            inPlaceDict.Add(textBoxes[i], true);
        }

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
        
        for (int i = 0; i < textBoxes.Length; i++)
        {
            Image textBubble = textBoxes[i].GetComponentInChildren<Image>();
            textBubble.color = Color.Lerp(textBubble.color, new Color(0.026f, 0.037f, 0.25f), 0.4f);

            if (Vector2.Distance(textBoxes[i].transform.position, positions[3].position) < .01f)
            {
                //swap target position
                targetPos = positions[2].position;
                textBoxes[i].transform.SetParent(sortOrder[2].transform);
            }
            //continue for all other positions
            else if (Vector2.Distance(textBoxes[i].transform.position, positions[2].position) < .01f)
            {
                //swap target position
                targetPos = positions[1].position;
                textBoxes[i].transform.SetParent(sortOrder[1].transform);
            }
            else if (Vector2.Distance(textBoxes[i].transform.position, positions[1].position) < .01f)
            {
                //swap target position
                targetPos = positions[0].position; 
                textBoxes[i].transform.SetParent(sortOrder[0].transform); 
            }
            else if (Vector2.Distance(textBoxes[i].transform.position, positions[0].position) < .01f)
            {
                //swap target position
                textBoxes[i].transform.position = positions[3].position;
                textBoxes[i].GetComponentInChildren<TextMeshProUGUI>().text = "";
                textBubble.enabled = false;
                textBubble.color = new Color(1f, 1f, 1f);
                textBoxes[i].transform.SetParent(sortOrder[3].transform);
                targetPos = positions[3].position;
            }
            else
            {
                inPlace = false;
                Debug.LogWarning("Cant find where to go");
            }

            StartCoroutine(moveUp(textBoxes[i], targetPos));
        }
    }

    private IEnumerator moveUp(GameObject textbox, Vector3 target)
    {
        while (textbox.transform.position != target)
        {
            inPlaceDict[textbox] = false;
            inPlace = false;
            textbox.transform.position = Vector3.MoveTowards(textbox.transform.position, target, speed * Time.deltaTime);
            yield return null;
        }

        inPlaceDict[textbox] = true;
        
        inPlace = true;
    }

    public bool isInPlace()
    {
        foreach (var textBox in inPlaceDict)
        {
            if (textBox.Value == false) return false;
        }

        return true;
    }

    public void ResetPos()
    {
        for(int i = 0; i < textBoxes.Length; i++)
        {
            textBoxes[i].transform.position = positions[i].position;
        }
    }



}
