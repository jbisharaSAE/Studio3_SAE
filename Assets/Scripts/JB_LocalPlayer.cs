using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Linq;

public class JB_LocalPlayer : NetworkBehaviour
{
    // reference to objects important to each player, their ships and grid
    public GameObject[] ships;
    public GameObject gridLayout;

    public int playerPrefabIndex = 0;

    [SyncVar]
    public int playerID;

    [SyncVar]
    public int shipConfirmIndex = 1;

    [SyncVar]
    public bool playerReady;
    
    // Start is called before the first frame update
    void Start()
    {

        JB_GameManager.FindPlayerObjects();
        
        //if local player, enable ship, otherwise turn them off
        if (this.isLocalPlayer)
        {
            // confirmed test, this code works
            //this.OnSetLocalVisibility(true);
            //foreach (GameObject ship in ships)
            //{
            //    ship.SetActive(true);
            //}

            CmdSetPlayerID(Convert.ToInt32(GetComponent<NetworkIdentity>().netId.Value));

            //CmdSetPlayerPrefabs(this.gameObject);
            
        }
        else
        {
            //this.OnSetLocalVisibility(false);
            //foreach (GameObject ship in ships)
            //{
            //    ship.SetActive(false);
            //}
        }
    }

    [Command]
    void CmdSetPlayerID(int netID)
    {
        playerID = netID;
        RpcOnSetPlayerID(playerID);

    }

    //[Command]
    //void CmdSetPlayerPrefabs(GameObject player)
    //{
    //    JB_GameManager.playerPrefabs[playerPrefabIndex] = player;
        
    //}

    [ClientRpc]
    void RpcOnSetPlayerID(int ID)
    {
        playerID = ID;
    }

    //[ClientRpc]
    //void RpcSetPlayerPrefabs(GameObject player)
    //{
    //    JB_GameManager.playerPrefabs[playerPrefabIndex] = player;
        
    //}

    
}
