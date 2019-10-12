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

    public TextMeshProUGUI textDisplayTest;

    [SyncVar]
    [HideInInspector]
    public int readyCheckNumber;

    public GameObject playerObj;

    private List<GameObject> players = new List<GameObject>();

    // reference to client
    private GameObject localPlayer;
    // find reference to the other player object
    private GameObject otherPlayer;

    [SerializeField]
    [Tooltip("Amount of dallions a player gets when it becomes their turn")]
    private float dallionsToAdd = 50f;

    private int counter;

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
