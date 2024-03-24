using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mapNode : MonoBehaviour
{
    [HideInInspector] public Vector2 anchoredPosition;
    public List<mapNode> pilgrimConnectedNodes, birdBeakConnectedNodes, galliumConnectedNodes;

    [Header("Setup")]
    [SerializeField] GameObject connectionPrefab;

    [Header("Settings")]
    public bool pilgrimLine;
    public bool birdBeakLine;
    public bool galliumLine;


    private void Awake()
    {
        anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    private void debugConnections()
    {
        /*
        for (int i = 0; i < connectedNodes.Count; i++)
        {
            GameObject connection = Instantiate(connectionPrefab, gameObject.transform);
            RectTransform connectionTransform = connection.GetComponent<RectTransform>();

            Vector2 angle = anchoredPosition - connectedNodes[i].anchoredPosition;
            connectionTransform.up = angle.normalized;

            float distance = Vector2.Distance(anchoredPosition, connectedNodes[i].anchoredPosition);
            connectionTransform.localScale = new Vector3(1, 5, 1);

            connection.transform.SetParent(gameObject.transform.parent, true);
            connection.transform.SetSiblingIndex(1);


        }
        */
    }

    
}
