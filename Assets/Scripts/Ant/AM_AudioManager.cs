using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AM_AudioManager : MonoBehaviour
{
    [SerializeField]
    //Blast Abilty SFX reference
    private AudioSource blastSFX;

    [SerializeField]
    //Barrage Ability SFX reference
    private AudioSource barrageSFX;

    [SerializeField]
    //Ship Death SFX reference
    private AudioSource shipDeathSFX;

    [SerializeField]
    //Player Lose SFX reference
    private AudioSource playerLoseSFX;

    [SerializeField]
    //Player Win SFX reference
    private AudioSource playerWinSFX;

    [SerializeField]
    //Shield spawn SFX reference
    private AudioSource shieldSpawnSFX;

    [SerializeField]
    //Shield death SFX reference
    private AudioSource shieldDeathSFX;

    // Start is called before the first frame update
    void Start()
    {
        //Get access to gameobject that holds the ability audio source,
        //As well as the audio source compoenent inside that game object;
        blastSFX = GameObject.Find("Blast").GetComponent<AudioSource>();

        //access to Barrage audio source
        barrageSFX = GameObject.Find("Barrage").GetComponent<AudioSource>();

        //access to Ship death audio source
        shipDeathSFX = GameObject.Find("Ship Death").GetComponent<AudioSource>();

        //access to Player Lose audio source
        playerLoseSFX = GameObject.Find("Ship Death").GetComponent<AudioSource>();

        //access to Player Win audio source
        playerWinSFX = GameObject.Find("Ship Death").GetComponent<AudioSource>();

        //access to Shield Spawn audio source
        shieldSpawnSFX = GameObject.Find("Shield Ability 1").GetComponent<AudioSource>();

        //access to Shield Despawn audio source
        shieldDeathSFX = GameObject.Find("Shield Ability 2").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
    }

    //Testing purposes only
    void CheckInput()
    {
        //Input for Blast Ability SFX
        if (Input.GetKeyDown(KeyCode.E) && !blastSFX.isPlaying)
        {
            PlayBlast();
        }

        //Input for Barrage Ability SFX
        if (Input.GetKeyDown(KeyCode.R) && !barrageSFX.isPlaying)
        {
            PlayBarrage();
        }

        //Input for Ship Death SFX
        if (Input.GetKeyDown(KeyCode.T) && !shipDeathSFX.isPlaying)
        {
            ShipDeath();
        }

        //Input for Player Lose SFX
        if (Input.GetKeyDown(KeyCode.Y) && !playerLoseSFX.isPlaying)
        {
            PlayerLose();
        }

        //Input for Player Win SFX
        if (Input.GetKeyDown(KeyCode.U) && !shieldDeathSFX.isPlaying)
        {
            PlayerWin();
        }

        //Input for Shield Activate SFX
        if (Input.GetKeyDown(KeyCode.I) && !shieldSpawnSFX.isPlaying)
        {
            ShieldSpawn();
        }

        //Input for Shield Death SFX
        if (Input.GetKeyDown(KeyCode.O) && !shieldDeathSFX.isPlaying)
        {
            ShieldDeath();
        }
    }

    //Trigger this function to play the SFX for BLAST ABILITY
    public void PlayBlast()
    {
        blastSFX.Play();
    }

    //Trigger this function to play the SFX for BARRAGE ABILITY
    public void PlayBarrage()
    {
        barrageSFX.Play();
    }

    //Trigger this function to play the SFX for SHIP DEATH
    public void ShipDeath()
    {
        shipDeathSFX.Play();
    }

    //Trigger this function to play the SFX for PLAYER LOSE
    public void PlayerLose()
    {
        playerLoseSFX.Play();
    }

    public void PlayerWin()
    {
        playerWinSFX.Play();
    }

    //Trigger this function to play the SFX for SHIELD SPAWN
    public void ShieldSpawn()
    {
        shieldSpawnSFX.Play();
    }

    public void ShieldDeath()
    {
        shieldDeathSFX.Play();
    }
}
