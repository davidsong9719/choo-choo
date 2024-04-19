using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dialogueAudio : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] List<AudioClip> playerClips, opponentClips;
    [SerializeField] List<float> playerVolumes, opponentVolumes;


    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void makeNoise(int speaker)
    {
        audioSource.Stop();

        audioSource.pitch = Random.Range(0.95f, 1.05f);
        if (speaker == 0)
        {
            int clipIndex = Random.Range(0, playerClips.Count);
            audioSource.volume = playerVolumes[clipIndex];
            audioSource.clip = playerClips[clipIndex];

        } else if (speaker == 1)
        {

            int clipIndex = Random.Range(0, opponentClips.Count);
            audioSource.volume = opponentVolumes[clipIndex];
            audioSource.clip = opponentClips[clipIndex];
        }



        audioSource.Play();
    }
}
