using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class JB_GameManager : NetworkBehaviour
{
    public static GameObject[] playerPrefabs;

    [SerializeField]
    private GameObject abilityButtons;

    private GameObject tempGridLayout;

    private bool isOriginal = false;

    public int readyCheckNumber;


    void Start()
    {

        GameObject[] all = GameObject.FindGameObjectsWithTag(this.tag);

        // to avoid duplicates of this game object when created, so only one exists in scene at all times
        if (all.Length > 1)
        {
            if (!isOriginal)
            {
                Destroy(this.gameObject);


            }
        }

        isOriginal = true;

    }

    // Update is called once per frame
    void Update()
    {
        if (readyCheckNumber == 2)
        {
            // start game
            StartGame();


        }
    }

    [Command]
    public void CmdCheckPlayerReady(int index)
    {
        Debug.Log("test when checkplayerready called");
        // if all of players ships are in valid position
        if (index >= 4)
        {


            // increment number, and game starts when variable reaches 2

                //CmdIncrementReadyNumber();

                //++readyCheckNumber;

                //RpcIncrementReadyNumber(readyCheckNumber);

            Debug.Log("ready check number is: " + readyCheckNumber);
        }
    }

    public static void FindPlayerObjects()
    {
        playerPrefabs = GameObject.FindGameObjectsWithTag("Player");
      

    }

    private void StartGame()
    {
       

        // swapping grid layout for players
        //tempGridLayout = playerPrefabs[0].GetComponent<JB_LocalPlayer>().gridLayoutPrefab;
        //playerPrefabs[0].GetComponent<JB_LocalPlayer>().gridLayoutPrefab = playerPrefabs[1].GetComponent<JB_LocalPlayer>().gridLayoutPrefab;
        //playerPrefabs[1].GetComponent<JB_LocalPlayer>().gridLayoutPrefab = tempGridLayout;

        // disable positioning buttons
       

        // enable ability buttons
        abilityButtons.GetComponentInChildren<GameObject>().SetActive(true);

    }

    public void AbilityOne(GameObject obj)
    {
        obj.SetActive(true);
    }




}
