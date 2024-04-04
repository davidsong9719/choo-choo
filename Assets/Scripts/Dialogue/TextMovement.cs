using System.Collections;
using TMPro;
using UnityEngine;

public class TextMovement : MonoBehaviour
{
    public Transform pos1, pos2, pos3, pos4;
    public int speed;

    [SerializeField] GameObject[] textBoxes;

    private Vector3 targetPos;

    private static TextMovement instance;


    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Found more than one Dialogue Manager in scene");
        }
        instance = this;

    }
    public static TextMovement GetInstance()
    {
        return instance;
    }

    public void moveBoxes()
    {
        for (int i = 0; i < textBoxes.Length; i++)
        {
            Debug.Log("moving " + textBoxes[i].name);
            if (Vector2.Distance(textBoxes[i].transform.position, pos4.position) < .1f)
            {
                //swap target position
                targetPos = pos3.position;
            }
            //continue for all other positions
            else if (Vector2.Distance(textBoxes[i].transform.position, pos3.position) < .1f)
            {
                //swap target position
                targetPos = pos2.position;
            }
            else if (Vector2.Distance(textBoxes[i].transform.position, pos2.position) < .1f)
            {
                //swap target position
                targetPos = pos1.position;
            }
            else if (Vector2.Distance(textBoxes[i].transform.position, pos1.position) < .1f)
            {
                //swap target position
                textBoxes[i].transform.position = pos4.position;
                textBoxes[i].GetComponentInChildren<TextMeshProUGUI>().text = "";
                targetPos = pos4.position;
            }
            else
            {
                Debug.LogWarning("Ya this aint anywhere rip");
            }
            StartCoroutine(moveUp(textBoxes[i], targetPos));
        }
        
    }

    private IEnumerator moveUp(GameObject textbox, Vector3 target)
    {
        while (textbox.transform.position != target)
        {
            textbox.transform.position = Vector3.MoveTowards(textbox.transform.position, target, speed * Time.deltaTime);
            yield return null;
        }
    }

}
