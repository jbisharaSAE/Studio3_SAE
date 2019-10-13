using System.Collections;
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

    public GameObject playerObj;

    [SerializeField]
    [Tooltip("Amount of dallions a player gets when it becomes their turn")]
    private float dallionsToAdd = 50f;



    
    public void ShipHit(ShipType ship)
    {

        Debug.Log("ship sent thru parameter = " + ship);
        switch (ship)
        {
            case ShipType.Ship1:
                --hitPoints[0];
                break;
            case ShipType.Ship2:
                --hitPoints[2];
                break;
            case ShipType.Ship3:
                --hitPoints[2];
                break;
            case ShipType.Ship4:
                --hitPoints[3];
                break;
            default:
                break;
        }

        ShipsRemaining();
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
            // game over - other player wins
        }
        
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

        foreach (KeyValuePair<NetworkInstanceId, NetworkIdentity> pair in NetworkServer.objects)
        {
            if (pair.Value.gameObject.tag == "Player")
            {
                if (pair.Value.gameObject.GetComponent<JB_LocalPlayer>().myTurn)
                {
                    CmdAddResourcesToPlayer(pair.Value.gameObject);
                }

            }
        }
    }

    [Command]
    void CmdAddResourcesToPlayer(GameObject playerObj)
    {
        playerObj.GetComponent<JB_LocalPlayer>().currentResources += dallionsToAdd;
    }


}
