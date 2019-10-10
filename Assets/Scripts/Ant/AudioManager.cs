using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //Blast Abilty SFX reference
    private GameObject blastPlayer;
    private AudioSource blastSFX;

    //Barrage Ability SFX reference
    private GameObject barragePlayer;
    private AudioSource barrageSFX;

    //Ship Death SFX reference
    private GameObject shipDeathPlayer;
    private AudioSource shipDeathSFX;

    //Player Lose SFX reference
    private GameObject playerLosePlayer;
    private AudioSource playerLoseSFX;

    //Player Win SFX reference
    private GameObject playerWinPlayer;
    private AudioSource playerWinSFX;

    //Shield spawn SFX reference
    private GameObject shieldSpawnPlayer;
    private AudioSource shieldSpawnSFX;

    //Shield death SFX reference
    private GameObject shieldDeathPlayer;
    private AudioSource shieldDeathSFX;

    // Start is called before the first frame update
    void Start()
    {
        //Get access to gameobject that holds the blast ability audio source,
        //As well as the audio source compoenent inside that game object
        blastPlayer = GameObject.Find("Blast");
        blastSFX = blastPlayer.GetComponent<AudioSource>();

        //access to Barrage audio source
        barragePlayer = GameObject.Find("Barrage");
        barrageSFX = barragePlayer.GetComponent<AudioSource>();

        //access to Ship death audio source
        shipDeathPlayer = GameObject.Find("Ship Death");
        shipDeathSFX = shipDeathPlayer.GetComponent<AudioSource>();

        //access to Player Lose audio source
        playerLosePlayer = GameObject.Find("Player Lose");
        playerLoseSFX = playerLosePlayer.GetComponent<AudioSource>();

        //access to Player Win audio source
        playerWinPlayer = GameObject.Find("Player Win");
        playerWinSFX = playerWinPlayer.GetComponent<AudioSource>();

        //access to Shield Spawn audio source
        shieldSpawnPlayer = GameObject.Find("Shield Ability 1");
        shieldSpawnSFX = shieldSpawnPlayer.GetComponent<AudioSource>();

        //access to Shield Despawn audio source
        shieldDeathPlayer = GameObject.Find("Shield Ability 2");
        shieldDeathSFX = shieldDeathPlayer.GetComponent<AudioSource>();
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
