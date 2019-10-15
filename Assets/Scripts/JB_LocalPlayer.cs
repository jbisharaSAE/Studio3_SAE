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

    // barrage projectile prefab
    public GameObject barrageProjectilePrefab;
    // barrage location prefab
    public GameObject barragePrefab;

    // barrage projectile spawn points
    private Vector3[] barrageSpawnPoint;

    // currency text display
    public GameObject dallionDisplay;

    // reference to objects important to each player, their ships and grid
    public GameObject[] shipPrefabs;
    public GameObject[] ships;
    public GameObject gridLayout;
    public GameObject gameManagerPrefab;
    private GameObject gameManager;

    // used for switch function, to determine which ability to use
    //private int abilityNumber = 5;
    public Button[] abilityButtons;
    public GameObject myButtons;
    

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
    [SyncVar]
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

    private Canvas overlayCanvas;
    private Text displayCurrentDallions;

    private List<GameObject> myList = new List<GameObject>();

   
    private void Awake()
    {
        // find the canvas in game (scene)
        overlayCanvas = GameObject.FindGameObjectWithTag("OverlayCanvas").GetComponent<Canvas>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // call method to find all player prefabs in scene
        JB_GameManager.FindPlayerObjects();

        //if local player, enable ship, otherwise turn them off
        if (!this.isLocalPlayer)
        {
            // exit if this is not local player
            return;
        }

        barrageSpawnPoint = new Vector3[4];

        //this.gameObject.tag = "LocalPlayer";

        //abilityButtons = new Button[5];
        isButtonHeld = new bool[4];

        //dallionDisplay.transform.SetParent(overlayCanvas.transform, false);

        dallionDisplay.SetActive(true);
        displayCurrentDallions = dallionDisplay.transform.GetChild(0).gameObject.GetComponent<Text>();
        //displayCurrentDallions.text = currentResources.ToString("F0");

        Debug.Log(displayCurrentDallions.text);
        // set the parent of myButtons game object to the canvas in game
        //myButtons.transform.SetParent(overlayCanvas.transform);

        // find my error message game object
        errorAlertTextObj = GameObject.FindGameObjectWithTag("ErrorMsg");
        

        gridLayout.SetActive(true);

        // converts the network ID given to player prefabs that spawn when a client joins the server into an integer
        // used to identify player's connection object from each other
        CmdSetPlayerID(Convert.ToInt32(GetComponent<NetworkIdentity>().netId.Value));

    }

    private void Update()
    {
        if (!this.isLocalPlayer) { return; }

        if (!myTurn) { Debug.LogWarning("It is not my turn"); return; }

        displayCurrentDallions.text = currentResources.ToString("F0");

        // test to see if we are in battle mode
        if (!showRotateConfirmButtons)
        {
            // players touch Input.Touch(0) ---  Input.touchCount > 0
            if (Input.GetMouseButtonDown(0))
            {
                // get information from player's touch on screen
                //Touch touch = Input.GetTouch(0);
                
                RaycastHit hit;                                // mouse is for testing
                if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit)) // shooting ray to mouse position
                {
                    if (hit.collider.gameObject.tag == "Tile") // did player click on a tile
                    {
                        Debug.Log("tile hit");
                        tempTargetPos = hit.collider.gameObject.GetComponent<JB_Tile>().tilePosition;

                        for (int i = 0; i < isButtonHeld.Length; ++i)
                        {
                            if (isButtonHeld[i])
                            {
                                ActivateAbilities(i);
                            }
                        }
                    }
                    else if(hit.collider.gameObject.tag == "Square")
                    {
                        Debug.Log("square hit");
                        tempTargetPos = hit.collider.gameObject.transform.position;          //GetComponent<JB_SquareSprites>().tileRef.GetComponent<JB_Tile>().tilePosition;

                        for (int i = 0; i < isButtonHeld.Length; ++i)
                        {
                            if (isButtonHeld[i])
                            {
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
                    // take away the resource cost of this ability
                    currentResources -= blastCost;

                    // ability one - blast
                    CmdAbilityOneBlast(tempTargetPos, currentResources);
                }
                
                // ability is no longer active
                isButtonHeld[0] = false;

                
                break;
            case 1:
                if(currentResources >= barrageCost)
                {
                    // take away the resource cost of this ability
                    currentResources -= barrageCost;

                    // ability two - barrage
                    CmdAbilityTwoBarrage(tempTargetPos, currentResources);
                }
                
                // ability is no longer active
                isButtonHeld[1] = false;

                break;
            case 2:
                // ability three - radar
                CmdAbilityThreeRadar();

                // ability is no longer active
                isButtonHeld[2] = false;

                // take away the resource cost of this ability
                currentResources -= radarCost;
                break;
            case 3:
                // ability four - shield
                CmdAbilityFourShield();

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
    // colliders on own player also disabled, to avoid targeting own ships, by accident or w/e
    public void DisableMyTileColliders()
    {   
        BoxCollider[] tileColliders = gridLayout.GetComponentsInChildren<BoxCollider>();
        GameObject[] allShips = GameObject.FindGameObjectsWithTag("Ship");

        foreach(BoxCollider collider in tileColliders)
        {
            collider.enabled = false;
        }

        foreach(GameObject ship in allShips)
        {
            if(ship.GetComponent<NetworkIdentity>().hasAuthority)
            {
                BoxCollider[] shipSquares = ship.GetComponentsInChildren<BoxCollider>();
                foreach (BoxCollider square in shipSquares)
                {
                    square.enabled = false;
                }
                
            }
            
        }

    }

   
    // =============================== functions to execute abilities ============================
    [Command]
    private void CmdAbilityOneBlast(Vector3 targetPos, float updateCurrency)
    {
        // assigning tile position from click to the variable trueTarget
        trueTarget = targetPos;

        // spawns my projectile
        GameObject projectile = Instantiate(blastProjectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
        
        // assigning variables on projectile
        projectile.GetComponent<AM_JB_BlastProjectile>().targetTilePos = trueTarget;
        projectile.GetComponent<AM_JB_BlastProjectile>().playerObj = this.gameObject;
        
        NetworkServer.SpawnWithClientAuthority(projectile, connectionToClient);

        currentResources = updateCurrency;

        RpcAbilityOneBlast(projectile, trueTarget, updateCurrency);


    }

    [ClientRpc]
    void RpcAbilityOneBlast(GameObject projectile, Vector3 targetPos, float updateCurrency)
    {
        // letting everyone know what those variables are
        projectile.GetComponent<AM_JB_BlastProjectile>().targetTilePos = targetPos;
        projectile.GetComponent<AM_JB_BlastProjectile>().playerObj = this.gameObject;
        currentResources = updateCurrency;
    }

    [Command]
    private void CmdAbilityTwoBarrage(Vector3 targetPos, float updateCurrency)
    {
        // assigning tile position from click to the variable trueTarget
        trueTarget = targetPos;

        // spawn barrage prefab
        GameObject barrage = Instantiate(barragePrefab, targetPos, Quaternion.identity);
        NetworkServer.SpawnWithClientAuthority(barrage, connectionToClient);

        // target locations for barrage projectiles
        //barrageSpawnPoint
        for (int i = 0; i < 4; ++i)
        {
            // first 4 child objects of barrage prefab (ie the projectiles)             // 154 is just above the screen
            barrageSpawnPoint[i] = new Vector3(barrage.transform.GetChild(i).position.x, 154.0f, barrage.transform.GetChild(i).position.z);  // GetComponent<JB_BarrageProjectile>().playerObj = this.gameObject;
            GameObject barrageProj = Instantiate(barrageProjectilePrefab, barrageSpawnPoint[i], Quaternion.identity);

            Vector3 pos = barrage.transform.GetChild(i).position;
            float delayTime;
            // assigning variables to the spawned barrage projectiles
            barrageProj.GetComponent<JB_BarrageProjectile>().targetPos = pos;
            barrageProj.GetComponent<JB_BarrageProjectile>().delayTime = delayTime = ((float)i);
            barrageProj.GetComponent<JB_BarrageProjectile>().playerObj = this.gameObject;

            NetworkServer.SpawnWithClientAuthority(barrageProj, connectionToClient);

            RpcSpawnBarrageProjectiles(barrageProj, pos, delayTime);
        }

        

        RpcAbilityTwoBarrage(barrage, updateCurrency);
    }

    [ClientRpc]
    private void RpcSpawnBarrageProjectiles(GameObject projectile, Vector3 targetPos, float delayTime)
    {
        projectile.GetComponent<JB_BarrageProjectile>().targetPos = targetPos;
        projectile.GetComponent<JB_BarrageProjectile>().delayTime = delayTime;
        projectile.GetComponent<JB_BarrageProjectile>().playerObj = this.gameObject;
    }

    [ClientRpc]
    private void RpcAbilityTwoBarrage(GameObject barrage, float updateCurrency)
    {
        currentResources = updateCurrency;

               
    }

    [Command]
    private void CmdAbilityThreeRadar()
    {
        

    }

    [Command]
    private void CmdAbilityFourShield()
    {

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

    
    public void FindShipHit(ShipType ship, GameObject shipObj, Vector3 squarePos)
    {
        // information for when a player hits an enemy ship
        gameManager.GetComponent<JB_GameManager>().ShipHit(ship, shipObj, squarePos);
    }

    // unused at the moment
    [Command]
    public void CmdIncrementReadyNumber()
    {
        // tell the server im ready


        if (runOnce)
        {
            foreach (KeyValuePair<NetworkInstanceId, NetworkIdentity> pair in NetworkServer.objects)
            {
                if (pair.Value.gameObject.tag == "GameManager")
                {
                    pair.Value.gameObject.GetComponent<JB_GameManager>().ReadyCheckNumber();
                }

            }
            runOnce = false;
        }
        
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
            ships[i].GetComponent<DragObject>().EnableShipSprite();
            NetworkServer.SpawnWithClientAuthority(ships[i], connectionToClient);
            RpcAssignShips(ships[i], i);
        }

        gameManager = Instantiate(gameManagerPrefab);

        NetworkServer.SpawnWithClientAuthority(gameManager, connectionToClient);

        RpcAssignAuthorityToGameManager(gameManager);

        //gameManager.GetComponent<NetworkIdentity>().AssignClientAuthority(this.connectionToClient);

        //gameManager = Instantiate(gameManagerPrefab);

        //NetworkServer.Spawn(gameManager);


    }

    [ClientRpc]
    void RpcAssignAuthorityToGameManager(GameObject obj)
    {
        gameManager = obj;
    }

    [ClientRpc]
    void RpcAssignShips(GameObject ship, int index)
    {
        //grab the ship prefabs attached to game object, assign them to the array of ships
        ships = new GameObject[shipPrefabs.Length];

        ships[index] = ship;
        ships[index].GetComponent<DragObject>().EnableShipSprite();
        // one for each ship
        checkValidation = new bool[4];
    }

    private void OnGUI()
    {


        if (showRotateConfirmButtons && this.isLocalPlayer)
        {
            // ================= PLACEMENT STAGE ================

            // confirm ship positions checks all at once ======= button
            if (GUI.Button(new Rect((Screen.width / 2) + 45, (Screen.height * 0.9f), 70, 50), "Confirm"))
            {
                // one for each ship
                checkValidation = new bool[4];


                GameObject[] allShips = GameObject.FindGameObjectsWithTag("Ship");
                
                // making sure ships belong to me (local player)
                foreach(GameObject ship in allShips)
                {
                    if(ship.GetComponent<DragObject>().playerID == playerID)
                    {
                        myList.Add(ship);
                    }
                }

                //Debug.Log(checkValidation.Length);

                for (int i = 0; i < myList.Count; ++i)
                {
                    checkValidation[i] = myList[i].GetComponent<JB_SnappingShip>().ValidPosition();
                }

                allTrue = checkValidation.All(x => x);

                if (allTrue)
                {
                   
                    CmdIncrementReadyNumber();

                    for (int i = 0; i < myList.Count; ++i)
                    {
                        myList[i].GetComponent<JB_SnappingShip>().FreeOrLockShipPosition(false);
                        myList[i].GetComponent<DragObject>().canDrag = false;
                    }


                }
                else
                {
                    myList.Clear();
                    StartCoroutine(SendErrorAlert());
                    Debug.Log("not in valid positions");
                }
            }

            // rotate ship that is selected ====== button
            if (GUI.Button(new Rect((Screen.width / 2) -45f, (Screen.height * 0.9f), 70, 50), "Rotate"))
            {
                // frees up tiles that were taken before rotating ship
                shipObj.GetComponent<JB_SnappingShip>().FreeOrLockShipPosition(true);

                RotateShip();

            }
        }

        // ================== GAME ATTACK PHASE ========================
        else if (this.isLocalPlayer && myTurn) // SHOW ABILITY BUTTONS
        {
            float screenY = Screen.height;
            float screenX = Screen.width;

            GUILayout.BeginArea(new Rect(screenY * 0.1f, screenY * 0.9f, screenX * 0.9f, screenY));
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("End Turn", GUILayout.Height(50))) // end turn button - new Rect(330, myHeight, 70, 25),
            {
                gameManager.GetComponent<JB_GameManager>().ChangePlayerTurn();
            }
            if (GUILayout.Button("Blast", GUILayout.Height(50))) // blast ability - new Rect(430, myHeight, 70, 25), 
            {
                isButtonHeld[0] = OnlyOneButton(0, isButtonHeld[0]);
                Debug.Log("ability one clicked!!! ======= :)" + isButtonHeld[0]);
            }
            if (GUILayout.Button("Barrage", GUILayout.Height(50))) // barrage ability - new Rect(450, myHeight, 70, 25), 
            {
                isButtonHeld[1] = OnlyOneButton(1, isButtonHeld[1]);
                Debug.Log("ability two clicked!!! ======= :)" + isButtonHeld[1]);
            }
            if (GUILayout.Button("Radar", GUILayout.Height(50))) // radar ability - new Rect(470, myHeight, 70, 25), 
            {
                isButtonHeld[2] = OnlyOneButton(2, isButtonHeld[2]);
                Debug.Log("ability three clicked!!! ======= :)" + isButtonHeld[2]);
            }
            if (GUILayout.Button("Shield", GUILayout.Height(50))) // shield ability - new Rect(490, myHeight, 70, 25), 
            {
                isButtonHeld[3] = OnlyOneButton(3, isButtonHeld[3]);
                Debug.Log("ability four clicked!!! ======= :)" + isButtonHeld[3]);
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

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

    public void GameOver()
    {
        errorAlertTextObj = GameObject.FindGameObjectWithTag("ErrorMsg");
        errorAlertTextObj.GetComponent<TextMeshProUGUI>().text = "Game Over!";
        errorAlertTextObj.GetComponent<TextMeshProUGUI>().enabled = true;
        this.enabled = false;
    }
    
}
