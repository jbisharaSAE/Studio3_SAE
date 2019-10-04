using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Linq;

public class JB_LocalPlayer : NetworkBehaviour
{
    // reference to objects important to each player, their ships and grid
    public GameObject[] shipPrefabs;
    public static GameObject[] ships;
    public GameObject gridLayoutPrefab;
    public GameObject gameManagerPrefab;

    private GameObject gridLayout;
    private GameObject gameManager;
    

    //public int playerPrefabIndex = 0;

    [SyncVar]
    public int playerID;

    //[SyncVar]
    //public int shipConfirmIndex = 1;

    [SyncVar]
    public bool playerReady; // currently unused
    
    // Start is called before the first frame update
    void Start()
    {
        //ships = new GameObject[shipPrefabs.Length];

        JB_GameManager.FindPlayerObjects();
        
        //if local player, enable ship, otherwise turn them off
        if (!this.isLocalPlayer)
        {
            
            // exit if this is not local player
            return;

        }

        // spawn ship prefabs in game
        //for(int i = 0; i < shipPrefabs.Length; ++i)
        //{
        //    ships[i] = Instantiate(shipPrefabs[i]);
        //}

        gridLayoutPrefab.SetActive(true);
        // converts the network ID given to player prefabs that spawn when a client joins the server into an integer
        // used to identify player's connection object from each other
        CmdSetPlayerID(Convert.ToInt32(GetComponent<NetworkIdentity>().netId.Value));
    }

    [Command]
    void CmdSetPlayerID(int netID)
    {
        playerID = netID;

        gameManager = Instantiate(gameManagerPrefab);

        NetworkServer.Spawn(gameManager);

        //gridLayout = Instantiate(gridLayoutPrefab);


        //NetworkServer.SpawnWithClientAuthority(gridLayout, connectionToClient);



    }

  

    //[ClientRpc]
    //void RpcSetPlayerPrefabs(GameObject player)
    //{
    //    JB_GameManager.playerPrefabs[playerPrefabIndex] = player;

    //}


}
