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
    public GameObject waitingOnPlayerSign;
    public GameObject placementAlertDisplay;
    public GameObject plusParticleEffect;
    public GameObject zoomControlPrefab;
    public GameObject namingPhase;
    public TextMeshProUGUI nameDisplay;
    public TextMeshProUGUI timerDisplay;
    public TextMeshProUGUI playerTurnDisplay;

    private GameObject zoomControl;
    
    // number to display for radar
    public GameObject radarDisplayPrefab;

    // spawn point for projectiles
    public Transform blastSpawnPoint;

    // projectile blast prefab
    public GameObject blastProjectilePrefab;

    // barrage projectile prefab
    public GameObject barrageProjectilePrefab;

    // area for barrage projectiles to fly to
    public GameObject barrageAreaPrefab;

    // spawn point for barrage projectiles
    public Transform barrageSpawnPoint;

    // volley projectile prefab
    public GameObject volleyProjectilePrefab;

    // volley location prefab
    public GameObject volleyAreaPrefab;

    // shield prefab
    public GameObject shieldPrefab;
    private GameObject shield;

    // radar prefab
    public GameObject radarPrefab;
    private GameObject radarObj;

    // the barrage projectile to instantiate
    private GameObject barrageProj;

    // the volley projectile to instantiate
    private GameObject volleyProj;

    // spawn points for volley projectile to fly to
    private GameObject volleyArea;

    // spawn points for barrage projectiles to fly to
    private GameObject barrageArea;

    // volley projectile spawn points
    private Vector3[] volleySpawnPoint;

    // currency text display
    public GameObject dallionDisplay;

    // reference to objects important to each player, their ships and grid
    public GameObject[] shipPrefabs;
    public GameObject[] ships;
    public GameObject gridLayout;
    public GameObject gameManagerPrefab;
    private GameObject gameManager;

    // a boolean for each ability button to determine if button is active or not
    [SerializeField] private bool[] isButtonHeld;

    // used to find all ships in scene
    private GameObject[] shipsInGame;

    // to make sure player does not run the function more than once
    private bool runOnce = true;

    //used to hide / show rotate / confirm buttons

    public bool showRotateConfirmButtons = false;
    public bool startTimer = false;

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
    public bool myTurn; 

    // UI text message to show error of placement of ships
    public GameObject errorAlertTextObj;

    // resources for players to spend shooting / using abilities
    [SyncVar]
    public float currentResources;
    private float maxResources;

    // resource cost for abilities
    [SerializeField] private float blastCost = 25f;
    [SerializeField] private float volleyCost = 70f;
    [SerializeField] private float radarCost = 50f;
    [SerializeField] private float shieldCost = 15f;
    [SerializeField] private float barrageCost = 100f;

    // ability canvas ui buttons
    [SerializeField] private Image[] abilityButtons;

    // disabled button image
    public Sprite offButton;
    public Sprite onButton;

    // ability buttons place holder
    public GameObject abilityButtonsHolder;

    // target tile position
    private Vector3 tempTargetPos;

    [SyncVar]
    private Vector3 trueTarget;

    private Text displayCurrentDallions;

    private List<GameObject> myList = new List<GameObject>();

    // used to calculate closest ship using grid
    public int startX;
    public int startY;

    // tiles closest to radar ping
    public int radarCount;

    // timer for player's turn
    [SyncVar]
    public float timer = 30f;
    private bool playerReady;


    // Start is called before the first frame update
    void Start()
    {
        // call method to find all player prefabs in scene
        JB_GameManager.FindPlayerObjects();

        // initiliasing size of array
        
        //if local player, enable ship, otherwise turn them off
        if (!this.isLocalPlayer)
        {
            // exit if this is not local player
            return;
        }

        volleySpawnPoint = new Vector3[4];

        isButtonHeld = new bool[5];
        //abilityButtons = new Image[5];

        

        // find my error message game object
        errorAlertTextObj = GameObject.FindGameObjectWithTag("ErrorMsg");

        CmdSpawnGameManager();

        namingPhase.SetActive(true);

        // converts the network ID given to player prefabs that spawn when a client joins the server into an integer
        // used to identify player's connection object from each other
        CmdSetPlayerID(Convert.ToInt32(GetComponent<NetworkIdentity>().netId.Value));

        CmdSpawnZoom();

        if (isServer)
        {
            RpcChangeBarrageSpawnPoint();
        }
        else
        {
            CmdChangeBarrageSpawnPoint();
        }


    }

    public void TurnOnButtons()
    {
        abilityButtonsHolder.SetActive(true);
    }

    [Command]
    private void CmdChangeBarrageSpawnPoint()
    {
        RpcChangeBarrageSpawnPoint();
    }

    [ClientRpc]
    private void RpcChangeBarrageSpawnPoint()
    {
        // checking to see if we are on the right side or left, to change the spawn location for our barrage missiles
        if (transform.position.x < 0)
        {
            float x = barrageSpawnPoint.position.x - 100f;
            barrageSpawnPoint.position = new Vector2(x, barrageSpawnPoint.position.y);
            barrageSpawnPoint.up = Vector2.right;
        }
        else
        {
            float x = 150f + barrageSpawnPoint.position.x;
            barrageSpawnPoint.position = new Vector2(x, barrageSpawnPoint.position.y);
            barrageSpawnPoint.up = Vector2.left;
        }
    }

    [Command]
    private void CmdSpawnZoom()
    {
        zoomControl = Instantiate(zoomControlPrefab);

        NetworkServer.SpawnWithClientAuthority(zoomControl, connectionToClient);
    }

    
    public void StartPlacementPhase()
    {
        if (!this.isLocalPlayer)
        {
            return;
        }
        

        StartCoroutine(PlacementAlertMsg());

        dallionDisplay.SetActive(true);
        displayCurrentDallions = dallionDisplay.transform.GetChild(0).gameObject.GetComponent<Text>();
        playerTurnDisplay.enabled = true;

        namingPhase.SetActive(false);

        showRotateConfirmButtons = true;

        gridLayout.SetActive(true);

        GameObject[] ships = GameObject.FindGameObjectsWithTag("Ship");

        for(int i = 0; i < ships.Length; ++i)
        {
            if (ships[i].GetComponent<NetworkIdentity>().hasAuthority)
            {
                ships[i].GetComponent<DragObject>().EnableShipSprite();
                //RpcAssignShips(ships[i], i, true);
            }
            
        }

        nameDisplay.text = playerName;
        
    }

    IEnumerator PlacementAlertMsg()
    {
        placementAlertDisplay.SetActive(true);
        yield return new WaitForSeconds(3.5f);
        placementAlertDisplay.SetActive(false);
    }

    public void ChangePlayerName(String n)
    {
        if (isServer)
        {
            RpcChangePlayerName(n);
        }
        else
        {
            CmdChangePlayerName(n);
        }
    }

    [Command]
    public void CmdChangePlayerName(string n)
    {

        RpcChangePlayerName(n);
        // change text game object to this playername
       
    }

    [ClientRpc]
    public void RpcChangePlayerName(string n)
    {
        playerName = n;
    }

    
    private void Update()
    {
        if (!this.isLocalPlayer) { return; }

        DisplayTimer();


        if (!myTurn)
        {
            playerTurnDisplay.text = "enemy turn";
            
        }
        else
        {
            playerTurnDisplay.text = "your turn";
            
        }


        if (!myTurn)
        {
            return;
        }

        displayCurrentDallions.text = currentResources.ToString("F0");

        PlayerInput();
        
        
    }

    public IEnumerator SpawnPlusParticle()
    {

        plusParticleEffect.SetActive(true);
        yield return new WaitForSeconds(2.5f);
        plusParticleEffect.SetActive(false);

    }

    private void DisplayTimer()
    {

        if (!showRotateConfirmButtons && startTimer)
        {
            timer -= Time.deltaTime;
            timerDisplay.text = timer.ToString("F1");

            if(timer <= 0)
            {
                
                timer = 30f;

                if (myTurn)
                {
                    gameManager.GetComponent<JB_GameManager>().ChangePlayerTurn();
                    FindOtherPlayer();
                }

            }

        }
        

        
    }
    #region input
    private void PlayerInput()
    {
        // test to see if we are in battle mode
        if (!showRotateConfirmButtons)
        {
            // players touch Input.Touch(0) ---  Input.touchCount > 0 && Input.touchCount < 2 ---Input.GetMouseButtonDown(0)
            if (Input.GetMouseButtonDown(0))
            {
                // get information from player's touch on screen
                //Touch touch = Input.GetTouch(0);

                RaycastHit hit;                                // mouse is for testing -- touch.position -- Input.mousePosition
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit)) // shooting ray to mouse position
                {
                    if (hit.collider.gameObject.tag == "Tile") // did player click on a tile
                    {
                        Debug.Log("tile hit, we clicked on a tile");
                        tempTargetPos = hit.collider.gameObject.GetComponent<JB_Tile>().tilePosition;

                        startX = hit.collider.gameObject.GetComponent<JB_Tile>().x;
                        startY = hit.collider.gameObject.GetComponent<JB_Tile>().y;

                        for (int i = 0; i < isButtonHeld.Length; ++i)
                        {
                            if (isButtonHeld[i])
                            {
                                ActivateAbilities(i);
                            }
                        }
                    }
                    else if (hit.collider.gameObject.tag == "Square")
                    {
                        Debug.Log("square hit, we have clicked on a ship");
                        tempTargetPos = hit.collider.gameObject.transform.position;        

                        for (int i = 0; i < isButtonHeld.Length; ++i)
                        {
                            if (isButtonHeld[i])
                            {
                                ActivateAbilities(i);
                            }
                        }
                    }
                    else if (hit.collider.gameObject.tag == "Shield")
                    {
                        tempTargetPos = hit.collider.gameObject.transform.position;
                        for (int i = 0; i < isButtonHeld.Length; ++i)
                        {
                            if (isButtonHeld[i])
                            {
                                ActivateAbilities(i);
                            }
                        }
                    }
                    
                    

                    //else if ()


                }
            }
        }
    }

    #endregion
    
    #region abilityCalls
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
                if(currentResources >= volleyCost)
                {
                    // take away the resource cost of this ability
                    currentResources -= volleyCost;

                    // ability two - volley
                    CmdAbilityTwoVolley(tempTargetPos, currentResources);
                }
                

                // ability is no longer active
                isButtonHeld[1] = false;

                break;
            case 2:
                if(currentResources >= radarCost)
                {
                    // take away the resource cost of this ability
                    currentResources -= radarCost;

                    // ability three - radar
                    CmdAbilityThreeRadar(tempTargetPos, currentResources);
                }
              


                // ability is no longer active
                isButtonHeld[2] = false;

                break;
            case 3:
                if(currentResources>= shieldCost)
                {
                    // take away the resource cost of this ability
                    currentResources -= shieldCost;

                    // ability four - shield
                    CmdAbilityFourShield(tempTargetPos, currentResources);
                }
               


                // ability is no longer active
                isButtonHeld[3] = false;
                SwapGridColliders(false);

                break;
            case 4:
                if(currentResources >= barrageCost)
                {
                    // take away the resource cost of this ability
                    currentResources -= barrageCost;

                    // ability five - barrage
                    CmdAbilityFiveBarrage(tempTargetPos, currentResources);
                }
               

                isButtonHeld[4] = false;

                break;
            default:
                break;
        }
    }
    #endregion

    // when the game starts, player still sees their grid, but colliders are disabled, to avoid aiming at own grid
    // colliders on own player also disabled, to avoid targeting own ships, by accident or w/e
    public void DisableMyTileColliders()
    {
        if (!this.isLocalPlayer)
        {
            return;
        }
        GameObject[] allShips = GameObject.FindGameObjectsWithTag("Ship");

        BoxCollider[] tileColliders = gridLayout.GetComponentsInChildren<BoxCollider>();
        

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

    public void ShipDetection(GameObject gridManagerObj, int endX, int endY)
    {
        gridManagerObj.GetComponent<JB_GridManager>().startX = startX;
        gridManagerObj.GetComponent<JB_GridManager>().startY = startY;
        gridManagerObj.GetComponent<JB_GridManager>().endX = endX;
        gridManagerObj.GetComponent<JB_GridManager>().endY = endY;

        if(startX == 0 && startY == 0 && endX == 0 && endY == 0)
        {
            radarCount = 0;
        }
        else
        {
            radarCount = gridManagerObj.GetComponent<JB_GridManager>().FindClosestShip();
        }
        

        GameObject radarNumber = Instantiate(radarDisplayPrefab, tempTargetPos, Quaternion.identity);
        radarNumber.GetComponentInChildren<TextMeshProUGUI>().text = radarCount.ToString();

        Destroy(radarNumber, 3f);


    }

    #region abilities

    // =============================== functions to execute abilities ============================
    [Command]
    private void CmdAbilityOneBlast(Vector3 targetPos, float updateCurrency)
    {
        // assigning tile position from click to the variable trueTarget
        trueTarget = targetPos;

        // spawns my projectile
        GameObject projectile = Instantiate(blastProjectilePrefab, blastSpawnPoint.position, Quaternion.identity);
        
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
    private void CmdAbilityTwoVolley(Vector3 targetPos, float updateCurrency)
    {
        volleySpawnPoint = new Vector3[4];

        // assigning tile position from click to the variable trueTarget
        trueTarget = targetPos;

        // spawn volley prefab
        volleyArea = Instantiate(volleyAreaPrefab, targetPos, Quaternion.identity);
        NetworkServer.SpawnWithClientAuthority(volleyArea, connectionToClient);

        // target locations for volley projectiles
        // volley area target locations
        for (int i = 0; i < 4; ++i)
        {
            // first 4 child objects of volley area prefab (ie the projectiles)         // 154 is just above the screen
            volleySpawnPoint[i] = new Vector3(volleyArea.transform.GetChild(i).position.x, 154.0f, volleyArea.transform.GetChild(i).position.z);  
            volleyProj = Instantiate(volleyProjectilePrefab, volleySpawnPoint[i], volleyProjectilePrefab.transform.rotation);

            Vector3 pos = volleyArea.transform.GetChild(i).position;
            float delayTime;
            // assigning variables to the spawned volley projectiles
            volleyProj.GetComponent<JB_VolleyProjectile>().targetPos = pos;
            volleyProj.GetComponent<JB_VolleyProjectile>().delayTime = delayTime = ((float)i /4);
            volleyProj.GetComponent<JB_VolleyProjectile>().playerObj = this.gameObject;

            NetworkServer.SpawnWithClientAuthority(volleyProj, connectionToClient);

            RpcSpawnVolleyProjectiles(volleyProj, pos, delayTime);
        }


        RpcAbilityTwoVolley(updateCurrency);
    }

    [ClientRpc]
    private void RpcSpawnVolleyProjectiles(GameObject projectile, Vector3 targetPos, float delayTime)
    {
        projectile.GetComponent<JB_VolleyProjectile>().targetPos = targetPos;
        projectile.GetComponent<JB_VolleyProjectile>().delayTime = delayTime;
        projectile.GetComponent<JB_VolleyProjectile>().playerObj = this.gameObject;
    }

    [ClientRpc]
    private void RpcAbilityTwoVolley(float updateCurrency)
    {
        currentResources = updateCurrency;

               
    }

    [Command]
    private void CmdAbilityThreeRadar(Vector3 targetPos, float updateCurrency)
    {
        currentResources = updateCurrency;

        // spawn radar prefab
        radarObj = Instantiate(radarPrefab, targetPos, Quaternion.identity);

        // assigning variables on instantiated object
        radarObj.GetComponent<JB_RadarScript>().playerObj = this.gameObject;

        NetworkServer.SpawnWithClientAuthority(radarObj, connectionToClient);

        RpcAbilityThreeRadar(radarObj, updateCurrency);

    }

    [ClientRpc]
    private void RpcAbilityThreeRadar(GameObject radar, float updateCurrency)
    {
        currentResources = updateCurrency;

        radar.GetComponent<JB_RadarScript>().playerObj = this.gameObject;
    }

    [Command]
    private void CmdAbilityFourShield(Vector3 targetPos, float updateCurrency)
    {
        shield = Instantiate(shieldPrefab, targetPos, Quaternion.identity);
        currentResources = updateCurrency;
        NetworkServer.Spawn(shield);
        RpcAbilityFourShield(shield, updateCurrency);


    }

    [ClientRpc]
    private void RpcAbilityFourShield(GameObject shieldObj, float updateCurrency)
    {
        shield = shieldObj;
        currentResources = updateCurrency;
    }

    [Command]
    private void CmdAbilityFiveBarrage(Vector3 targetPos, float updateCurrency)
    {
        // assigning tile position from click to the variable trueTarget
        //trueTarget = targetPos;

        // spawn barrage prefab
        barrageArea = Instantiate(barrageAreaPrefab, targetPos, Quaternion.identity);
        NetworkServer.SpawnWithClientAuthority(barrageArea, connectionToClient);

        // target locations for barrage projectiles
        //barrageSpawnPoint
        for (int i = 0; i < 5; ++i)
        {
            float rotZ = UnityEngine.Random.Range(barrageSpawnPoint.eulerAngles.z - 10, barrageSpawnPoint.eulerAngles.z + 10);
            
            // need the same rotation of the spawn point
            //Quaternion newRot = new Quaternion(0f, 0f, rotZ, 1f);
            

            barrageProj = Instantiate(barrageProjectilePrefab, barrageSpawnPoint.position, barrageSpawnPoint.rotation);

            Vector3 pos = barrageArea.transform.GetChild(i).position;

            float delayTime;

            // assigning variables to the spawned barrage projectiles
            barrageProj.GetComponent<JB_BarrageProjectile>().targetPos = pos;
            barrageProj.GetComponent<JB_BarrageProjectile>().delayTime = delayTime = ((float)i / 4);
            barrageProj.GetComponent<JB_BarrageProjectile>().playerObj = this.gameObject;
            barrageProj.transform.Rotate(0f, 0f, rotZ);

            NetworkServer.SpawnWithClientAuthority(barrageProj, connectionToClient);

            RpcSpawnBarrageProjectiles(barrageProj, pos, delayTime, rotZ);
        }


        RpcAbilityFiveBarrage(updateCurrency);
    }

    [ClientRpc]
    void RpcSpawnBarrageProjectiles(GameObject projectile, Vector3 targetPos, float delayTime, float z)
    {
        projectile.GetComponent<JB_BarrageProjectile>().targetPos = targetPos;
        projectile.GetComponent<JB_BarrageProjectile>().delayTime = delayTime;
        projectile.GetComponent<JB_BarrageProjectile>().playerObj = this.gameObject;
        projectile.transform.Rotate(0f, 0f, z);

    }

    [ClientRpc]
    void RpcAbilityFiveBarrage(float updateCurrency)
    {
        currentResources = updateCurrency;
    }

    #endregion
    // =============================== functions to execute abilities ============================



    // method used to ensure only one ability is active any one time
    public bool OnlyOneButton(int index, bool bToChange)
    {
        bToChange = !bToChange;

        for(int i = 0; i < isButtonHeld.Length; ++i)
        {
            isButtonHeld[i] = false;
            abilityButtons[i].sprite = offButton;
        }
        
        isButtonHeld[index] = bToChange;

        if (bToChange)
        {
            abilityButtons[index].sprite = onButton;
        }
        else
        {
            abilityButtons[index].sprite = offButton;
        }

        return bToChange;
    }

    
    public void FindShipHit(ShipType ship, GameObject shipObj, Vector2 squarePos)
    {
        // information for when a player hits an enemy ship
        gameManager.GetComponent<JB_GameManager>().ShipHit(ship, shipObj, squarePos);
    }

    
    [Command]
    public void CmdIncrementReadyNumber()
    {
        // tell the server im ready

        // setting up error message for not enough resources to use ability
        //errorAlertTextObj.GetComponent<TextMeshProUGUI>().text = "Not enough dallions!";
        
        // find my error message game object
        errorAlertTextObj = GameObject.FindGameObjectWithTag("ErrorMsg");

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
            ships[i].GetComponent<DragObject>().enabled = true;
            ships[i].GetComponent<DragObject>().playerID = netID;
            NetworkServer.SpawnWithClientAuthority(ships[i], connectionToClient);
            RpcAssignShips(ships[i], i);
        }

       

    }

    [Command]
    void CmdSpawnGameManager()
    {
        gameManager = Instantiate(gameManagerPrefab);

        NetworkServer.SpawnWithClientAuthority(gameManager, connectionToClient);

        RpcAssignAuthorityToGameManager(gameManager);
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

    }

    [Command]
    void CmdClearList()
    {
        myList.Clear();
    }

    private void OnGUI()
    {
        if (showRotateConfirmButtons && this.isLocalPlayer && !playerReady)
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

                    playerReady = true;

                    for (int i = 0; i < myList.Count; ++i)
                    {
                        myList[i].GetComponent<JB_SnappingShip>().FreeOrLockShipPosition(false);
                        myList[i].GetComponent<DragObject>().canDrag = false;
                    }


                }
                else
                {
                    CmdClearList();
                    myList.Clear();
                    StartCoroutine(SendErrorAlert());
                    Debug.Log("not in valid positions");
                }

                if (playerReady)
                {
                    waitingOnPlayerSign.SetActive(true);
                    abilityButtonsHolder.SetActive(true);
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
        //else if (this.isLocalPlayer && myTurn) // SHOW ABILITY BUTTONS
        //{
        //    float screenY = Screen.height;
        //    float screenX = Screen.width;

        //    GUILayout.BeginArea(new Rect(screenY * 0.1f, screenY * 0.9f, screenX * 0.9f, screenY));
        //    GUILayout.BeginHorizontal();
        //    if (GUILayout.Button("End Turn", GUILayout.Height(50))) // end turn button - new Rect(330, myHeight, 70, 25),
        //    {
        //        gameManager.GetComponent<JB_GameManager>().ChangePlayerTurn();
        //    }
        //    GUILayout.Space(25f);
        //    if (GUILayout.Button("Blast / 25", GUILayout.Height(50))) // blast ability - new Rect(430, myHeight, 70, 25), 
        //    {
        //        if(currentResources >= blastCost)
        //        {
        //            isButtonHeld[0] = OnlyOneButton(0, isButtonHeld[0]);
        //        }
        //        else
        //        {
        //            StartCoroutine(NotEnoughMoney());
        //        }
                
        //        Debug.Log("ability one clicked!!! ======= :)" + isButtonHeld[0]);
        //    }
        //    if (GUILayout.Button("Volley / 70", GUILayout.Height(50))) // volley ability - new Rect(450, myHeight, 70, 25), 
        //    {
        //        if (currentResources >= volleyCost)
        //        {
        //            isButtonHeld[1] = OnlyOneButton(1, isButtonHeld[1]);
        //        }
        //        else
        //        {
        //            StartCoroutine(NotEnoughMoney());
        //        }

        //        Debug.Log("ability two clicked!!! ======= :)" + isButtonHeld[1]);
        //    }
        //    if (GUILayout.Button("Radar / 50", GUILayout.Height(50))) // radar ability - new Rect(470, myHeight, 70, 25), 
        //    {
        //        if (currentResources >= radarCost)
        //        {
        //            isButtonHeld[2] = OnlyOneButton(2, isButtonHeld[2]);
        //        }
        //        else
        //        {
        //            StartCoroutine(NotEnoughMoney());
        //        }

        //        Debug.Log("ability three clicked!!! ======= :)" + isButtonHeld[2]);
        //    }
        //    if (GUILayout.Button("Shield / 15", GUILayout.Height(50))) // shield ability - new Rect(490, myHeight, 70, 25), 
        //    {
        //        if (currentResources >= shieldCost)
        //        {
        //            isButtonHeld[3] = OnlyOneButton(3, isButtonHeld[3]);
        //        }
        //        else
        //        {
        //            StartCoroutine(NotEnoughMoney());
        //        }

        //        SwapGridColliders(isButtonHeld[3]);
        //        Debug.Log("ability four clicked!!! ======= :)" + isButtonHeld[3]);
        //    }
        //    if (GUILayout.Button("Barrage / 100", GUILayout.Height(50))) // barrage ability - new Rect(490, myHeight, 70, 25), 
        //    {
        //        if (currentResources >= barrageCost)
        //        {
        //            isButtonHeld[4] = OnlyOneButton(4, isButtonHeld[4]);
        //        }
        //        else
        //        {
        //            StartCoroutine(NotEnoughMoney());
        //        }

        //        Debug.Log("ability four clicked!!! ======= :)" + isButtonHeld[4]);
        //    }
        //    GUILayout.EndHorizontal();
        //    GUILayout.EndArea();

        //}
       
    }

    // toggle ability on booleans
    public void ToggleAbilityBool(int index)
    {
        if (!this.isLocalPlayer)
        {
            return;
        }

        if (myTurn)
        {
            switch (index)
            {
                // ability one
                case 0:
                    if (currentResources >= blastCost)
                    {
                        isButtonHeld[0] = OnlyOneButton(0, isButtonHeld[0]);
                    }
                    else
                    {
                        StartCoroutine(NotEnoughMoney());
                    }

                    Debug.Log("ability one clicked!!! ======= :)" + isButtonHeld[0]);

                    break;
                // ability two
                case 1:
                    if (currentResources >= volleyCost)
                    {
                        isButtonHeld[1] = OnlyOneButton(1, isButtonHeld[1]);
                    }
                    else
                    {
                        StartCoroutine(NotEnoughMoney());
                    }

                    Debug.Log("ability two clicked!!! ======= :)" + isButtonHeld[1]);

                    break;
                // ability three
                case 2:
                    if (currentResources >= radarCost)
                    {
                        isButtonHeld[2] = OnlyOneButton(2, isButtonHeld[2]);
                    }
                    else
                    {
                        StartCoroutine(NotEnoughMoney());
                    }

                    Debug.Log("ability three clicked!!! ======= :)" + isButtonHeld[2]);

                    break;
                // ability four
                case 3:
                    if (currentResources >= shieldCost)
                    {
                        isButtonHeld[3] = OnlyOneButton(3, isButtonHeld[3]);
                    }
                    else
                    {
                        StartCoroutine(NotEnoughMoney());
                    }

                    SwapGridColliders(isButtonHeld[3]);
                    Debug.Log("ability four clicked!!! ======= :)" + isButtonHeld[3]);

                    break;
                // ability five
                case 4:
                    if (currentResources >= barrageCost)
                    {
                        isButtonHeld[4] = OnlyOneButton(4, isButtonHeld[4]);
                    }
                    else
                    {
                        StartCoroutine(NotEnoughMoney());
                    }

                    Debug.Log("ability four clicked!!! ======= :)" + isButtonHeld[4]);

                    break;
                // end turn
                case 5:
                    gameManager.GetComponent<JB_GameManager>().ChangePlayerTurn();
                    FindOtherPlayer();
                    break;
                default:
                    break;
            }
        }
        
    }

    private void FindOtherPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag(this.tag);

        foreach(GameObject player in players)
        {
            if (!player.GetComponent<NetworkIdentity>().isLocalPlayer)
            {
                StartCoroutine(SpawnPlusParticle());

            }
        }
    }
    
    private void SwapGridColliders(bool onOff)
    {

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
         
        foreach(GameObject player in players)
        {
            if (player.GetComponent<JB_LocalPlayer>().playerID == playerID)
            {
                BoxCollider[] tiles = player.transform.GetChild(0).GetComponentsInChildren<BoxCollider>();
                Debug.Log("Tiles array length = " + tiles.Length);
                Debug.Log("my player");
                foreach (BoxCollider tile in tiles)
                {
                    tile.enabled = onOff;
                    
                }
            }
            else
            {
                Debug.Log("other Player");
                BoxCollider[] tiles = player.transform.GetChild(0).GetComponentsInChildren<BoxCollider>();
                foreach (BoxCollider tile in tiles)
                {
                    tile.enabled = !onOff;
                    
                }
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

    IEnumerator NotEnoughMoney()
    {
        errorAlertTextObj.GetComponent<TextMeshProUGUI>().text = "Not enough dallions!";
        errorAlertTextObj.GetComponent<TextMeshProUGUI>().enabled = true;
        yield return new WaitForSeconds(3f);
        errorAlertTextObj.GetComponent<TextMeshProUGUI>().enabled = false;
    }

    private void OnDisconnectedFromServer(NetworkIdentity info)
    {
        foreach (KeyValuePair<NetworkInstanceId, NetworkIdentity> pair in NetworkServer.objects)
        {
            NetworkServer.Destroy(pair.Value.gameObject);
        }
    }

    
}
