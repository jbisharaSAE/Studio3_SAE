﻿using System.Collections;
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

    public TextMeshProUGUI textDisplayTest;

    private bool isOriginal = false;
    private bool runOnce = false;

    [SyncVar]
    public int readyCheckNumber;

    public GameObject playerObj;

    void Start()
    {
        // a boolean for each ability button
        //isButtonHeld = new bool[4];

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
            if (!runOnce)
            {
                StartGame();
                runOnce = true;
            }


        }
    }


    public static void FindPlayerObjects()
    {
        playerPrefabs = GameObject.FindGameObjectsWithTag("Player");
    }

    [Command]
    void CmdShowGrid(GameObject playerObj)
    {
        // displaying enemy and player grid
        playerObj.GetComponent<JB_LocalPlayer>().gridLayout.SetActive(true);

        // hiding rotate / confirm buttons
        playerObj.GetComponent<JB_LocalPlayer>().showRotateConfirmButtons = false;

        // enable ability buttons
        abilityButtons.SetActive(true);

        RpcShowGrid(playerObj);

        // start the method that find the ability buttons
        playerObj.GetComponent<JB_LocalPlayer>().RpcFindAbilityButtons();

        // calls a function to disable tile colliders locally
        playerObj.GetComponent<JB_LocalPlayer>().DisableMyTileColliders();

    }

    [ClientRpc]
    void RpcShowGrid(GameObject playerObj)
    {
        playerObj.GetComponent<JB_LocalPlayer>().gridLayout.SetActive(true);
        playerObj.GetComponent<JB_LocalPlayer>().showRotateConfirmButtons = false;

        // enable ability buttons
        abilityButtons.SetActive(true);
    }

    
    private void StartGame()
    {
        foreach (KeyValuePair<NetworkInstanceId, NetworkIdentity> pair in NetworkServer.objects)
        {
            if(pair.Value.gameObject.tag == "Player")
            {
                CmdShowGrid(pair.Value.gameObject);
                
            }
        }

        CmdSetPlayerTurn();

        // disable positioning buttons
        Debug.Log("============= game has started! ===============");
    }

    [Command]
    void CmdSetPlayerTurn()
    {
        if (!hasAuthority)
        {
            Debug.Log("does not have authority");
            return;
            
        }

        playerPrefabs[0].GetComponent<JB_LocalPlayer>().myTurn = true;
        playerPrefabs[0].GetComponent<JB_LocalPlayer>().currentResources = 50f;
        playerPrefabs[1].GetComponent<JB_LocalPlayer>().myTurn = false;
    }

    public void ChangePlayerTurn()
    {
        CmdChangePlayerTurn();
    }

    [Command]
    void CmdChangePlayerTurn()
    {
        playerPrefabs[0].GetComponent<JB_LocalPlayer>().myTurn = !playerPrefabs[0].GetComponent<JB_LocalPlayer>().myTurn;
        playerPrefabs[1].GetComponent<JB_LocalPlayer>().myTurn = !playerPrefabs[1].GetComponent<JB_LocalPlayer>().myTurn;
    }

    public void AbilityOne()
    {
        // toggles the ability button
        //isButtonHeld[0] = !isButtonHeld[0];

        textDisplayTest.text = "ability one activated";

        Debug.Log(playerObj.GetComponent<JB_LocalPlayer>().playerID);
    }




}
