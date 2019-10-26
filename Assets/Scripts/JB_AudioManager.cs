using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JB_AudioManager : MonoBehaviour
{
    public AudioClip menuMusic;
    public AudioClip gameMusic;

    private AudioSource myAudioSource;
    private bool isOriginal = false;

    void Start()
    {
        GameObject[] all = GameObject.FindGameObjectsWithTag(this.tag);

        if (all.Length > 1)
        {
            if (!isOriginal)
            {
                Destroy(gameObject);
            }
        }

        isOriginal = true;
        DontDestroyOnLoad(gameObject);

        myAudioSource = GetComponent<AudioSource>();
    }

    private void OnLevelWasLoaded(int level)
    {
        if (level == 0)
        {
            myAudioSource.clip = menuMusic;
            myAudioSource.Play();
        }
        else
        {
            if(myAudioSource.clip != gameMusic)
            {
                myAudioSource.clip = gameMusic;
                myAudioSource.Play();
            }
            
        }
    }





}
