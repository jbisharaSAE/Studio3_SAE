﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using System.Linq;


public enum ShipType { Ship1, Ship2, Ship3, Ship4 };

public class JB_GameManager : NetworkBehaviour
{
    private bool[] isShipDead = new bool[4];
    public int[] hitPoints = { 9, 6, 4, 6 };
    
    // testing to see if all hitpoint are 0
    private bool allTrue;

    public static GameObject[] playerPrefabs;

    [SyncVar]
    [HideInInspector]
    public int readyCheckNumber;

    public GameObject shipDestroyedParticle;
    public GameObject shipHitParicle;

    [SerializeField]
    [Tooltip("Amount of dallions a player gets when it becomes their turn")]
    private float dallionsToAdd = 50f;



    
    public void ShipHit(ShipType ship, GameObject shipObj, Vector3 squarePos)
    {

        Debug.Log("ship sent thru parameter = " + ship);
        switch (ship)
        {
            case ShipType.Ship1:
                --hitPoints[0];
                CmdSpawnShipHitParticle(squarePos);
                CmdTestShipLife(hitPoints[0], shipObj);
                break;
            case ShipType.Ship2:
                --hitPoints[1];
                CmdSpawnShipHitParticle(squarePos);
                CmdTestShipLife(hitPoints[1], shipObj);
                break;
            case ShipType.Ship3:
                --hitPoints[2];
                CmdSpawnShipHitParticle(squarePos);
                CmdTestShipLife(hitPoints[2], shipObj);
                break;
            case ShipType.Ship4:
                --hitPoints[3];
                CmdSpawnShipHitParticle(squarePos);
                CmdTestShipLife(hitPoints[3], shipObj);
                break;
            default:
                break;
        }

        ShipsRemaining();
    }

    [Command]
    void CmdTestShipLife(float hitPoints, GameObject shipObj)
    {
        if(hitPoints == 0)
        {
            GameObject go = Instantiate(shipDestroyedParticle, shipObj.transform.position, Quaternion.identity);

            NetworkServer.Spawn(go);

        }
    }

    [Command]
    void CmdSpawnShipHitParticle(Vector3 squarePos)
    {
        GameObject go = Instantiate(shipHitParicle, squarePos, Quaternion.identity);

        NetworkServer.Spawn(go);
    }


    void ShipsRemaining()
    {
        for(int i = 0; i < hitPoints.Length; ++i)
        {
            if(hitPoints[i] <= 0)
            {
                isShipDead[i] = true;
            }
        }

        allTrue = isShipDead.All(x => x);

        if (allTrue)
        {
            // game over
            CmdGameOver();
        }
        
    }

    [Command]
    void CmdGameOver()
    {
        foreach(GameObject player in playerPrefabs)
        {
            player.GetComponent<JB_LocalPlayer>().GameOver();
            RpcGameOver(player);
        }
    }

    [ClientRpc]
    void RpcGameOver(GameObject playerObj)
    {
        playerObj.GetComponent<JB_LocalPlayer>().GameOver();
    }

   
    public override void OnStartAuthority()
    {

        if (hasAuthority == false)
        {
            Destroy(this);
            return;
        }

        
    }

    public void ReadyCheckNumber()
    {
        ++readyCheckNumber;

        if(readyCheckNumber == 2)
        {
            StartGame();
        }
    }

    public static void FindPlayerObjects()
    {
        playerPrefabs = GameObject.FindGameObjectsWithTag("Player");
    }

    [Command]
    void CmdBeginPlay(GameObject playerObj)
    {
        // displaying enemy and player grid
        playerObj.GetComponent<JB_LocalPlayer>().gridLayout.SetActive(true);

        // hiding rotate / confirm buttons
        playerObj.GetComponent<JB_LocalPlayer>().showRotateConfirmButtons = false;

        // calls a function to disable tile colliders locally
        playerObj.GetComponent<JB_LocalPlayer>().DisableMyTileColliders();

        RpcBeginPlay(playerObj);

        // start the method that find the ability buttons
        //playerObj.GetComponent<JB_LocalPlayer>().RpcFindAbilityButtons();

        

    }

    [ClientRpc]
    void RpcBeginPlay(GameObject playerObj)
    {
        playerObj.GetComponent<JB_LocalPlayer>().gridLayout.SetActive(true);
        playerObj.GetComponent<JB_LocalPlayer>().showRotateConfirmButtons = false;
        playerObj.GetComponent<JB_LocalPlayer>().DisableMyTileColliders();
    }

    
    private void StartGame()
    {
        foreach (KeyValuePair<NetworkInstanceId, NetworkIdentity> pair in NetworkServer.objects)
        {
            if(pair.Value.gameObject.tag == "Player")
            {
                CmdBeginPlay(pair.Value.gameObject);

            }
        }
        
        //CmdSwapShipAuthority();

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

        if (playerPrefabs[0].GetComponent<JB_LocalPlayer>().myTurn)
        {
            CmdAddResourcesToPlayer(playerPrefabs[0]);  // add dallions to this player
        }
        else
        {
            CmdAddResourcesToPlayer(playerPrefabs[1]);  // add dallions to this player
        }

        //foreach (KeyValuePair<NetworkInstanceId, NetworkIdentity> pair in NetworkServer.objects)
        //{
        //    if (pair.Value.gameObject.tag == "Player")
        //    {
        //        if (pair.Value.gameObject.GetComponent<JB_LocalPlayer>().myTurn)
        //        {
        //            CmdAddResourcesToPlayer(pair.Value.gameObject);
        //        }

        //    }
        //}
    }

    [Command]
    void CmdAddResourcesToPlayer(GameObject playerObj)
    {
        if(playerObj.GetComponent<JB_LocalPlayer>().currentResources < 250)
        {
            playerObj.GetComponent<JB_LocalPlayer>().currentResources += dallionsToAdd;
            RpcAddResourcesToPlayer(playerObj, playerObj.GetComponent<JB_LocalPlayer>().currentResources);
        }
        
    }

    [ClientRpc]
    void RpcAddResourcesToPlayer(GameObject playerObj, float amount)
    {
        playerObj.GetComponent<JB_LocalPlayer>().currentResources = amount;
    }


}
