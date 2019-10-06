using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Linq;
using TMPro;
using UnityEngine.UI;

public class JB_LocalPlayer : NetworkBehaviour
{
    // testing for loop i iteration
    public int index;

    // reference to objects important to each player, their ships and grid
    public GameObject[] shipPrefabs;
    public GameObject[] ships;
    public GameObject gridLayoutPrefab;
    public GameObject gameManagerPrefab;

    private GameObject gridLayout;
    private GameObject gameManager;

    // used to find all ships in scene
    private GameObject[] shipsInGame;

    // to make sure player does not run the function more than once
    private bool runOnce = true;

    //used to reset the tiles when ship rotates
    private bool showRotateConfirmButtons;

    //used to check validation of ship locations
    private bool[] checkValidation;
    private bool allTrue;

    // current ship that is selected
    public static GameObject shipObj;

    
    [SyncVar]
    public string playerName;

    [SyncVar]
    public int playerID;
    
    [SyncVar]
    public bool playerReady; // currently unused

    public GameObject errorAlertTextObj;

    // Start is called before the first frame update
    void Start()
    {
        // find my error message game object
        errorAlertTextObj = GameObject.FindGameObjectWithTag("ErrorMsg");
        
        // call method to find all player prefabs in scene
        JB_GameManager.FindPlayerObjects();

        //if local player, enable ship, otherwise turn them off
        if (!this.isLocalPlayer)
        {
            // exit if this is not local player
            return;
        }

        //spawn ship prefabs in game
        

        gridLayoutPrefab.SetActive(true);

        gameManager = GameObject.FindGameObjectWithTag("GameManager");

       


        // converts the network ID given to player prefabs that spawn when a client joins the server into an integer
        // used to identify player's connection object from each other
        CmdSetPlayerID(Convert.ToInt32(GetComponent<NetworkIdentity>().netId.Value));
        


    }


    // unused at the moment
    [Command]
    public void CmdIncrementReadyNumber()
    {
        if (runOnce)
        {
            foreach (KeyValuePair<NetworkInstanceId, NetworkIdentity> pair in NetworkServer.objects)
            {
                if (pair.Value.gameObject.tag == "GameManager")
                {
                    //pair.Value.gameObject.GetComponent<NetworkIdentity>().AssignClientAuthority(this.GetComponent<NetworkIdentity>().connectionToServer);
                    pair.Value.gameObject.GetComponent<JB_GameManager>().readyCheckNumber++;
                }

            }
            runOnce = false;
        }
        Debug.Log("Increment Function Called");
    }

    [Command]
    void CmdSetPlayerID(int netID)
    {
        playerID = netID;

        //grab the ship prefabs attached to game object, assign them to the array of ships
        ships = new GameObject[shipPrefabs.Length];

        // one for each ship
        checkValidation = new bool[4];

        for (int i = 0; i < shipPrefabs.Length; ++i)
        {
            // instantiate ships and give them authority from this local player (client)
            ships[i] = Instantiate(shipPrefabs[i]);
            ships[i].GetComponent<DragObject>().playerID = netID;
            NetworkServer.SpawnWithClientAuthority(ships[i], connectionToClient);
            RpcAssignShips(ships[i], i);
        }

        //gameManager.GetComponent<NetworkIdentity>().AssignClientAuthority(this.connectionToClient);

        //gameManager = Instantiate(gameManagerPrefab);

        //NetworkServer.Spawn(gameManager);


    }

    [ClientRpc]
    void RpcAssignShips(GameObject ship, int index)
    {
        //grab the ship prefabs attached to game object, assign them to the array of ships
        ships = new GameObject[shipPrefabs.Length];

        ships[index] = ship;

        // one for each ship
        checkValidation = new bool[4];
    }

    private void OnGUI()
    {
        if (showRotateConfirmButtons)
        {


            // confirm ship positions checks all at once ======= button
            if (GUI.Button(new Rect(570, 500, 70, 25), "Confirm"))
            {
                // one for each ship
                checkValidation = new bool[4];

                ships = GameObject.FindGameObjectsWithTag("Ship");
                Debug.Log(checkValidation.Length);
                for (int i = 0; i < ships.Length; ++i)
                {
                    checkValidation[i] = ships[i].GetComponent<JB_SnappingShip>().ValidPosition();
                    Debug.Log("inside for loop: " + checkValidation[i] + ", " + index);
                    index = i;
                }

                Debug.Log("outside for loop: " + checkValidation + ", " + index + ", " + ships.Length);
                allTrue = checkValidation.All(x => x);

                if (allTrue)
                {
                    GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

                    foreach (GameObject player in players)
                    {
                        if (player.GetComponent<NetworkIdentity>().isLocalPlayer)
                            player.GetComponent<JB_LocalPlayer>().CmdIncrementReadyNumber();


                    }



                    for (int i = 0; i < ships.Length; ++i)
                    {
                        ships[i].GetComponent<JB_SnappingShip>().FreeOrLockShipPosition(false);
                        ships[i].GetComponent<DragObject>().canDrag = false;
                    }


                }
                else
                {
                    StartCoroutine(SendErrorAlert());
                    Debug.Log("not in valid positions");
                }
            }

            // rotate ship that is selected ====== button
            if (GUI.Button(new Rect(430, 500, 70, 25), "Rotate"))
            {
                // frees up tiles that were taken before rotating ship
                shipObj.GetComponent<JB_SnappingShip>().FreeOrLockShipPosition(true);

                RotateShip();

            }
        }
    }

    public void RotateShip()
    {
        // if shipObj is not empty, rotate 90 degrees
        if (shipObj != null)
        {
            shipObj.transform.Rotate(0f, 0f, 90f);
            shipObj.GetComponent<JB_SnappingShip>().ShipPlacement();

        }

    }

    IEnumerator SendErrorAlert()
    {
        // alerting player that their ship placement is invalid
        errorAlertTextObj.GetComponent<TextMeshProUGUI>().enabled = true;
        yield return new WaitForSeconds(4f);
        errorAlertTextObj.GetComponent<TextMeshProUGUI>().enabled = false;
    }

    
}
