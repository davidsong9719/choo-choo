using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class rolloverEffect : MonoBehaviour
{
    private Image imageComponent;
    // Start is called before the first frame update
    void Start()
    {
        imageComponent = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void spriteTo(Sprite newSprite)
    {
        if (imageComponent)
        {
            imageComponent.sprite = newSprite;
        }
    }
}
