using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class JB_GameManager : NetworkBehaviour
{
    public static GameObject[] playerPrefabs;
       
    [SyncVar]
    private int readyCheckNumber;

    public GameObject[] ships;

    [SerializeField]
    private GameObject abilityButtons;

    [SerializeField]
    private GameObject rotateConfirmButtons;

    private GameObject tempGridLayout;


   
    // Update is called once per frame
    void Update()
    {
        if(readyCheckNumber == 2)
        {
            // start game
            StartGame();

            
        }
    }

    public void CheckPlayerReady(int index)
    {
        Debug.Log("test when checkplayerready called");
        // if all of players ships are in valid position
        if(index == 4)
        {
            Debug.Log("Testing if statement");
            // increment number, and game starts when variable reaches 2
            CmdIncrementReadyNumber();
            Debug.Log("ready check number is: " + readyCheckNumber);
        }
    }

    [Command]
    void CmdIncrementReadyNumber()
    {
        ++readyCheckNumber;
        RpcIncrementReadyNumber(readyCheckNumber);
    }

    void RpcIncrementReadyNumber(int n)
    {
        readyCheckNumber = n;
    }

    public static void FindPlayerObjects()
    {
        playerPrefabs = GameObject.FindGameObjectsWithTag("Player");
        
    }

    private void StartGame()
    {
        for (int i = 0; i < playerPrefabs.Length; ++i)
        {
            for(int j = 0; j < playerPrefabs[i].GetComponent<JB_LocalPlayer>().shipPrefabs.Length; ++j)
            {
                // disabling ship objects on each player
                playerPrefabs[i].GetComponent<JB_LocalPlayer>().shipPrefabs[j].SetActive(false);
            }
        }

        // swapping grid layout for players
        tempGridLayout = playerPrefabs[0].GetComponent<JB_LocalPlayer>().gridLayoutPrefab;
        playerPrefabs[0].GetComponent<JB_LocalPlayer>().gridLayoutPrefab = playerPrefabs[1].GetComponent<JB_LocalPlayer>().gridLayoutPrefab;
        playerPrefabs[1].GetComponent<JB_LocalPlayer>().gridLayoutPrefab = tempGridLayout;

        // disable positioning buttons
        rotateConfirmButtons.SetActive(false);

        // enable ability buttons
        abilityButtons.SetActive(true);

    }

    public void AbilityOne(GameObject obj)
    {
        obj.SetActive(true);
    }
}
