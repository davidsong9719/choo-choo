using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class cardLayout : MonoBehaviour
{
    private enum displayInfo
    {
        playerDeck,
        drawPile,
        discardPile
    }


    [Header("Setup")]
    [SerializeField] List<Image> displayObject;
    [SerializeField] Sprite emptySprite;

    [Header("Settings")]
    [SerializeField] displayInfo displayContext;


    private void OnEnable()
    {
        refreshDisplay();
    }
    public void refreshDisplay()
    {
        List<Sprite> sprites = new List<Sprite>();

        switch (displayContext)
        {
            case displayInfo.playerDeck:
                sprites = copyToSpriteList(gameManager.instance.playerDeck, false);
                break;

            case displayInfo.drawPile:
                sprites = copyToSpriteList(combatManager.instance.drawPile, true);
                break;

            case displayInfo.discardPile:
                sprites = copyToSpriteList(combatManager.instance.discardPile, true);
                break;
        } 


        for (int i = 0; i < displayObject.Count; i++)
        {
            if (i >= sprites.Count)
            {
                displayObject[i].sprite = emptySprite;
            } else
            {
                displayObject[i].sprite = sprites[i];
            }
        }
    }

    private List<Sprite> copyToSpriteList(List<card> targetList, bool shuffle)
    {
        List<Sprite> newList = new List<Sprite>();

        for (int i = 0; i < targetList.Count; i++)
        {
            newList.Add(targetList[i].image);
        }

        if (!shuffle) return newList;

        List<Sprite> shuffledNewList = new List<Sprite>();

        while (newList.Count > 0)
        {
            int randomIndex = Random.Range(0, newList.Count);
            shuffledNewList.Add(newList[randomIndex]);
            newList.RemoveAt(randomIndex);
        }

        return shuffledNewList;
    }
}
