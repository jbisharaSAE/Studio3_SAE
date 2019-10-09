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
    // spawn point for projectiles
    public Transform projectileSpawnPoint;

    // projectile blast prefab
    public GameObject blastProjectilePrefab;

    // reference to objects important to each player, their ships and grid
    public GameObject[] shipPrefabs;
    public GameObject[] ships;
    public GameObject gridLayout;
    private GameObject gameManager;

    // used for switch function, to determine which ability to use
    private int abilityNumber = 5;
    private Button[] abilityButtons;

    // a boolean for each ability button to determine if button is active or not
    private bool[] isButtonHeld;

    // used to find all ships in scene
    private GameObject[] shipsInGame;

    // to make sure player does not run the function more than once
    private bool runOnce = true;

    //used to hide / show rotate / confirm buttons
    [HideInInspector]
    public bool showRotateConfirmButtons = true;

    //used to check validation of ship locations
    private bool[] checkValidation;
    private bool allTrue;

    // current ship that is selected
    public static GameObject shipObj;

    [SyncVar]
    public string playerName; // currently unused

    [SyncVar]
    public int playerID;
    
    [SyncVar]
    public bool myTurn; // currently unused

    // UI text message to show error of placement of ships
    public GameObject errorAlertTextObj;

    // resources for players to spend shooting / using abilities
    public float currentResources;
    private float maxResources;

    // resource cost for abilities
    [SerializeField]
    private float blastCost = 25f;
    [SerializeField]
    private float barrageCost = 75f;
    [SerializeField]
    private float radarCost = 50f;
    [SerializeField]
    private float shieldCost = 50f;

    // target tile position
    private Vector3 tempTargetPos;

    [SyncVar]
    private Vector3 trueTarget;
    
    // Start is called before the first frame update
    void Start()
    {
        abilityButtons = new Button[5];
        isButtonHeld = new bool[4];
        gameManager = GameObject.FindGameObjectWithTag("GameManager");

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


        gridLayout.SetActive(true);

        // converts the network ID given to player prefabs that spawn when a client joins the server into an integer
        // used to identify player's connection object from each other
        CmdSetPlayerID(Convert.ToInt32(GetComponent<NetworkIdentity>().netId.Value));

    }

    private void Update()
    {
        if (!this.isLocalPlayer) { return; }

        if (!myTurn) { Debug.LogWarning("It is not my turn"); return; }
        
        
        // test to see if we are in battle mode
        if (!showRotateConfirmButtons)
        {
            // players touch Input.Touch(0)
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;                                // mouse is for testing
                if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit)) // shooting ray to mouse position
                {
                    if(hit.collider.gameObject.tag == "Tile") // did player click on a tile
                    {
                        tempTargetPos = hit.collider.gameObject.GetComponent<JB_Tile>().tilePosition;

                        for(int i = 0; i < isButtonHeld.Length; ++i)
                        {
                            if (isButtonHeld[i])
                            {
                                abilityNumber = i;
                                ActivateAbilities(i);
                            }
                        }
                    }
                    // need an else if for detecting shield - TODO
                    // neeed an else if for detecting ship - TODO
                    //else if ()
                    
                    
                }
            }
        }

        
    }

    private void ActivateAbilities(int index)
    {
        switch (index)
        {
            case 0:
                if(currentResources >= blastCost)
                {
                    // ability one - blast
                    CmdAbilityOneBlast(tempTargetPos);

                    // take away the resource cost of this ability
                    currentResources -= blastCost;
                }
                
                // we only want to run this ability once at a time
                abilityNumber = 5;

                // ability is no longer active
                isButtonHeld[0] = false;

                
                break;
            case 1:
                // ability two - barrage
                CmdAbilityTwoBarrage();

                // we only want to run this ability once at a time
                abilityNumber = 5;

                // ability is no longer active
                isButtonHeld[1] = false;

                // take away the resource cost of this ability
                currentResources -= barrageCost;
                break;
            case 2:
                // ability three - radar
                CmdAbilityThreeRadar();

                // we only want to run this ability once at a time
                abilityNumber = 5;

                // ability is no longer active
                isButtonHeld[2] = false;

                // take away the resource cost of this ability
                currentResources -= radarCost;
                break;
            case 3:
                // ability four - shield
                CmdAbilityFourShield();

                // we only want to run this ability once at a time
                abilityNumber = 5;

                // ability is no longer active
                isButtonHeld[3] = false;

                // take away the resource cost of this ability
                currentResources -= shieldCost;
                break;
            default:
                break;
        }
    }

    // when the game starts, player still sees their grid, but colliders are disabled, to avoid aiming at own grid
    public void DisableMyTileColliders()
    {
        BoxCollider[] tileColliders = gridLayout.GetComponentsInChildren<BoxCollider>();

        foreach(BoxCollider collider in tileColliders)
        {
            collider.enabled = false;
        }

    }

    [ClientRpc]
    public void RpcFindAbilityButtons()
    {
        if (!myTurn)
        {
            return;
        }

        GameObject abilityButtonInScene = GameObject.Find("myButtons");
        abilityButtons = abilityButtonInScene.GetComponentsInChildren<Button>();

        abilityButtons[0].onClick.AddListener(AbilityOneToggle);      // ability button one
        abilityButtons[1].onClick.AddListener(AbilityTwoToggle);      // ability button two
        abilityButtons[2].onClick.AddListener(AbilityThreeToggle);    // ability button three
        abilityButtons[3].onClick.AddListener(AbilityFourToggle);     // ability button four
        abilityButtons[4].onClick.AddListener(EndTurn);               // end turn button
    }

    private void OnEnable()
    {
        if (!myTurn)
        {
            return;
        }

        if (this.isLocalPlayer && !showRotateConfirmButtons)
        {
            // assigning functions to each ability UI button
            abilityButtons[0].onClick.AddListener(AbilityOneToggle);
            abilityButtons[1].onClick.AddListener(AbilityTwoToggle);
            abilityButtons[2].onClick.AddListener(AbilityThreeToggle);
            abilityButtons[3].onClick.AddListener(AbilityFourToggle);
            abilityButtons[4].onClick.AddListener(EndTurn);

        }
    }

    private void OnDisable()
    {
        if (!myTurn)
        {
            return;
        }

        // removelisteners to avoid cpu usage, this is for UI buttons
        if (this.isLocalPlayer && !showRotateConfirmButtons)
        {
            abilityButtons[0].onClick.RemoveAllListeners();
            abilityButtons[1].onClick.RemoveAllListeners();
            abilityButtons[2].onClick.RemoveAllListeners();
            abilityButtons[3].onClick.RemoveAllListeners();
            abilityButtons[4].onClick.RemoveAllListeners();
        }

    }

    // =============================== toggle functions to determine if button is active or not ============================
    private void AbilityOneToggle()
    {
        isButtonHeld[0] = OnlyOneButton(0, isButtonHeld[0]);
        Debug.Log("ability one clicked!!! ======= :)" + isButtonHeld[0]);
    }

    private void AbilityTwoToggle()
    {
        isButtonHeld[1] = OnlyOneButton(1, isButtonHeld[1]);
        Debug.Log("ability two clicked!!! ======= :)" + isButtonHeld[1]);
    }

    private void AbilityThreeToggle()
    {
        isButtonHeld[2] = OnlyOneButton(2, isButtonHeld[2]);
        Debug.Log("ability three clicked!!! ======= :)" + isButtonHeld[2]);
    }

    private void AbilityFourToggle()
    {
        isButtonHeld[3] = OnlyOneButton(3, isButtonHeld[3]);
        Debug.Log("ability four clicked!!! ======= :)" + isButtonHeld[3]);
    }

    
    // =============================== toggle functions to determine if button is active or not ============================

    private void EndTurn()
    {
        Debug.Log("End Turn Clicked!! ++++++++++++++++");
        gameManager.GetComponent<JB_GameManager>().ChangePlayerTurn();
    }
    
        
    // =============================== functions to execute abilities ============================
    [Command]
    private void CmdAbilityOneBlast(Vector3 targetPos)
    {
        trueTarget = targetPos;

        GameObject projectile = Instantiate(blastProjectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
        
        projectile.GetComponent<BlastProjectile>().targetTilePos = trueTarget;
        Debug.Log(projectile.GetComponent<BlastProjectile>().targetTilePos);

        NetworkServer.SpawnWithClientAuthority(projectile, connectionToClient);
        
        
        RpcAbilityOneBlast(projectile, trueTarget);


    }

    [ClientRpc]
    void RpcAbilityOneBlast(GameObject projectile, Vector3 targetPos)
    {
        Debug.Log(projectile.GetComponent<BlastProjectile>().targetTilePos);

        projectile.GetComponent<BlastProjectile>().targetTilePos = targetPos;
    }

    [Command]
    private void CmdAbilityTwoBarrage()
    {

    }

    [Command]
    private void CmdAbilityThreeRadar()
    {
        

    }

    [Command]
    private void CmdAbilityFourShield()
    {
        // to ensure this method only gets called once
        abilityNumber = 5;
    }
    // =============================== functions to execute abilities ============================




    // method used to ensure only one ability is active any one time
    private bool OnlyOneButton(int index, bool bToChange)
    {
        bToChange = !bToChange;
        for(int i = 0; i < isButtonHeld.Length; ++i)
        {
            isButtonHeld[i] = false;
        }

        isButtonHeld[index] = bToChange;

        return bToChange;
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
        if (showRotateConfirmButtons && this.isLocalPlayer)
        {

            // confirm ship positions checks all at once ======= button
            if (GUI.Button(new Rect(570, 500, 70, 25), "Confirm"))
            {
                // one for each ship
                checkValidation = new bool[4];

                ships = GameObject.FindGameObjectsWithTag("Ship");
                //Debug.Log(checkValidation.Length);

                for (int i = 0; i < ships.Length; ++i)
                {
                    checkValidation[i] = ships[i].GetComponent<JB_SnappingShip>().ValidPosition();
                }

                allTrue = checkValidation.All(x => x);

                if (allTrue)
                {
                   
                    CmdIncrementReadyNumber();

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
