using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameManager : MonoBehaviour
{
    public static gameManager instance { get; private set; }

    [Header("Public Access")]
    public List<card> playerDeck;
    public List<card> effectCardTemplates;
    public int playerMaxHealth, playerHealth, playerSpeed, playerAttack, playerCash;
    public int followerAmount;
    public int timeElapsed;

    [Header("Templates")]
    public card attack, defend;
    public GameObject attackCardPrefab, defendCardPrefab, effectCardPrefab, nullCardPrefab;
    public AnimationCurve lerpCurve;

    [Header("Parameters")]
    public int stageOneLength;
    public int stageTwoLength;
    public int stageThreeLength;

    private float volume;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            cardGenerator.instance = GetComponent<cardGenerator>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void playSFX(AudioClip clip)
    {
        //CALL setVolume FIRST!!!!!
        //
        //

        GameObject sfxObject = new GameObject();
        AudioSource sfxSource = sfxObject.AddComponent<AudioSource>();
        sfxSource.clip = clip;
        sfxSource.volume = volume;
        sfxSource.Play();

        StartCoroutine(endSFX(sfxSource));
    }

    public void setVolume(float newVolume)
    {
        volume = newVolume;
    }

    IEnumerator endSFX(AudioSource sfxSource)
    {
        while (true)
        {
            if (sfxSource.isPlaying)
            {
                yield return null;
            } else
            {
                Destroy(sfxSource.gameObject);
                break;
            }
            
        }
    }
}
