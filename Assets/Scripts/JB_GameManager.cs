using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class JB_GameManager : NetworkBehaviour
{
    public static GameObject[] playerPrefabs;

    [SyncVar]
    public int readyCheckNumber;

    // currently unused
    public GameObject[] ships;

    [SerializeField]
    private GameObject abilityButtons;

    [SerializeField]
    private GameObject rotateConfirmButtons;

    private GameObject tempGridLayout;

    private bool isOriginal = false;

    public static GameObject shipObj;
    public GameObject errorAlertTextObj;
    public int placementIndex = 0;

    private Button rotateButton;
    private Button confirmButton;

    private void Awake()
    {
        rotateButton = GameObject.Find("ButtonRotate").GetComponent<Button>();
        confirmButton = GameObject.Find("ButtonConfirm").GetComponent<Button>();
    }
    void Start()
    {

        errorAlertTextObj = GameObject.Find("Text(TMP) ErrorShipPositionAlert");
        rotateConfirmButtons = GameObject.Find("EGO ButtonHolderPositionStage");
        abilityButtons = GameObject.Find("EGO ButtonHolderAbilities");

    

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

        GameObject rotConfirm = GameObject.Find("EGO RotateConfirmScript");
        //rotConfirm.GetComponent<JB_RotateConfirm>().FindGameManagerObj();
        //JB_RotateConfirm.gameManagerScript = gameObject.GetComponent<JB_GameManager>();

    }



    private void OnEnable()
    {

        rotateButton.onClick.AddListener(delegate () { RotateShip(); });
        confirmButton.onClick.AddListener(delegate () { ConfirmPosition(); });


    }

    private void OnDisable()
    {

        rotateButton.onClick.RemoveListener(delegate () { RotateShip(); });
        confirmButton.onClick.RemoveListener(delegate () { ConfirmPosition(); });


    }

    // Update is called once per frame
    void Update()
    {
        if (readyCheckNumber == 2)
        {
            // start game
            StartGame();


        }
    }

    [Command]
    public void CmdCheckPlayerReady(int index)
    {
        Debug.Log("test when checkplayerready called");
        // if all of players ships are in valid position
        if (index >= 4)
        {


            // increment number, and game starts when variable reaches 2

                //CmdIncrementReadyNumber();

                //++readyCheckNumber;

                //RpcIncrementReadyNumber(readyCheckNumber);

            Debug.Log("ready check number is: " + readyCheckNumber);
        }
    }

    [Command]
    void CmdIncrementReadyNumber()
    {
        ++readyCheckNumber;
        RpcIncrementReadyNumber(readyCheckNumber);
    }

    [ClientRpc]
    void RpcIncrementReadyNumber(int n)
    {
        readyCheckNumber = n;
    }

    public static void FindPlayerObjects()
    {
        playerPrefabs = GameObject.FindGameObjectsWithTag("Player");
      

    }



    private void StartGame()
    {
        for (int i = 0; i < playerPrefabs.Length; ++i)
        {
            for (int j = 0; j < playerPrefabs[i].GetComponent<JB_LocalPlayer>().shipPrefabs.Length; ++j)
            {
                // disabling ship objects on each player
                playerPrefabs[i].GetComponent<JB_LocalPlayer>().shipPrefabs[j].SetActive(false);
            }
        }

        // swapping grid layout for players
        //tempGridLayout = playerPrefabs[0].GetComponent<JB_LocalPlayer>().gridLayoutPrefab;
        //playerPrefabs[0].GetComponent<JB_LocalPlayer>().gridLayoutPrefab = playerPrefabs[1].GetComponent<JB_LocalPlayer>().gridLayoutPrefab;
        //playerPrefabs[1].GetComponent<JB_LocalPlayer>().gridLayoutPrefab = tempGridLayout;

        // disable positioning buttons
        rotateConfirmButtons.SetActive(false);

        // enable ability buttons
        abilityButtons.GetComponentInChildren<GameObject>().SetActive(true);

    }

    public void AbilityOne(GameObject obj)
    {
        obj.SetActive(true);
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

    public void FindGameManagerObj()
    {
        // find the game manager gameobject in scene
        //gameManagerScript = GameObject.FindGameObjectWithTag("GameManager").GetComponent<JB_GameManager>();
    }

    public void ConfirmPosition()
    {
        CmdSetAuthority();

        if (shipObj != null)
        {
            shipObj.SendMessage("LockShipPosition");

            if (shipObj.GetComponent<JB_SnappingShip>().ValidPosition()) // double checks that ship is valid position on the grid 
            {
                // if true we can lock the ship's position
                shipObj.GetComponent<DragObject>().canDrag = false;

                // index counter unique to each player to determine when game starts
                ++placementIndex;

                // checks if player has all ships in position and is ready to play
                CmdCheckPlayerReady(placementIndex);

                Debug.Log("Confirm Position Method, placement index: " + placementIndex);

            }
            else
            {
                // if not, the ship can still be moved by player
                shipObj.GetComponent<DragObject>().canDrag = true;
                StartCoroutine(SendErrorAlert());
            }
        }


    }

    IEnumerator SendErrorAlert()
    {
        // alerting player that their ship placement is invalid
        errorAlertTextObj.GetComponent<TextMeshProUGUI>().enabled = true;
        yield return new WaitForSeconds(4f);
        errorAlertTextObj.GetComponent<TextMeshProUGUI>().enabled = false;
    }

    [Command]
    void CmdSetAuthority( )
    {
        foreach (GameObject player in playerPrefabs)
        {
            this.GetComponent<NetworkIdentity>().AssignClientAuthority(player.GetComponent<NetworkIdentity>().connectionToClient);
        }
    }
}
