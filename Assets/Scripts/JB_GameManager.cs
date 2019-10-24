using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine.SceneManagement;

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

    [SyncVar]
    public int checkNumber;

    [SerializeField] private GameObject shipDestroyedPrefab;
    [SerializeField] private GameObject shipHitPrefab;

    private GameObject shipDestroy;
    private GameObject shipHit;

    [SerializeField]
    [Tooltip("Amount of dallions a player gets when it becomes their turn")]
    private float dallionsToAdd = 50f;


    public void ShipHit(ShipType ship, GameObject shipObj, Vector2 squarePos)
    {

        // taking hp off a ship
        
        switch (ship)
        {
            case ShipType.Ship1:
                --hitPoints[0];
                CmdSpawnShipHitParticle(squarePos);
                CmdTestShipLife(hitPoints[0], shipObj, ship);
                break;
            case ShipType.Ship2:
                --hitPoints[1];
                CmdSpawnShipHitParticle(squarePos);
                CmdTestShipLife(hitPoints[1], shipObj, ship);
                break;
            case ShipType.Ship3:
                --hitPoints[2];
                CmdSpawnShipHitParticle(squarePos);
                CmdTestShipLife(hitPoints[2], shipObj, ship);
                break;
            case ShipType.Ship4:
                --hitPoints[3];
                CmdSpawnShipHitParticle(squarePos);
                CmdTestShipLife(hitPoints[3], shipObj, ship);
                break;
            default:
                break;
        }

        ShipsRemaining();
    }

    [Command]
    void CmdTestShipLife(float hitPoints, GameObject shipObj, ShipType shipType)
    {
        if(hitPoints == 0)
        {
            List<Transform> squares = new List<Transform>();

            for(int i = 0; i < (shipObj.transform.childCount -1); ++i)
            {
                squares.Add(shipObj.transform.GetChild(i));
            }

            StartCoroutine(SpawnDestroyParticle(squares));


            foreach (GameObject player in playerPrefabs)
            {
                if (!player.GetComponent<NetworkIdentity>().hasAuthority)
                {
                    player.GetComponentInChildren<JB_GridManager>().ShowCross(shipType);
                }
            }
        }

    }

    

    private IEnumerator SpawnDestroyParticle(List<Transform> squareList)
    {
        foreach (Transform square in squareList)
        {
            shipDestroy = Instantiate(shipDestroyedPrefab, square.position, Quaternion.identity);
            NetworkServer.Spawn(shipDestroy);
            yield return new WaitForSeconds(0.2f);
        }

        
    }


    [Command]
    void CmdSpawnShipHitParticle(Vector3 squarePos)
    {
        shipHit = Instantiate(shipHitPrefab, squarePos, Quaternion.identity);

        NetworkServer.Spawn(shipHit);
        RpcSpawnShipHitParticle(shipHit);


    }

    [ClientRpc]
    void RpcSpawnShipHitParticle(GameObject particle)
    {
        shipHit = particle;
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
            //CmdGameOver();
            SceneManager.LoadScene(2);
        }
        
    }

    //[Command]
    //void CmdGameOver()
    //{
    //    foreach(GameObject player in playerPrefabs)
    //    {
    //        player.GetComponent<JB_LocalPlayer>().GameOver();
    //        RpcGameOver(player);
    //    }
    //}

    //[ClientRpc]
    //void RpcGameOver(GameObject playerObj)
    //{
    //    playerObj.GetComponent<JB_LocalPlayer>().GameOver();
    //}

   
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

        // showing mini ship icons
        playerObj.GetComponentInChildren<JB_GridManager>().ShowMiniShips();

        // hiding rotate / confirm buttons
        playerObj.GetComponent<JB_LocalPlayer>().showRotateConfirmButtons = false;

        // calls a function to disable tile colliders locally
        playerObj.GetComponent<JB_LocalPlayer>().DisableMyTileColliders();

        // displaying player's name
        playerObj.GetComponent<JB_LocalPlayer>().nameDisplay.text = playerObj.GetComponent<JB_LocalPlayer>().playerName;

        // start timer
        playerObj.GetComponent<JB_LocalPlayer>().timer = 30f;
        playerObj.GetComponent<JB_LocalPlayer>().startTimer = true;

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
        playerObj.GetComponent<JB_LocalPlayer>().nameDisplay.text = playerObj.GetComponent<JB_LocalPlayer>().playerName;
        playerObj.GetComponent<JB_LocalPlayer>().timer = 30f;
        playerObj.GetComponent<JB_LocalPlayer>().startTimer = true;
        playerObj.GetComponentInChildren<JB_GridManager>().ShowMiniShips();
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

        CmdSetPlayerTurn();

    }



    [Command]
    void CmdSetPlayerTurn()
    {
        if (!hasAuthority)
        {
            
            return;
            
        }

        // begin player turns using booleans
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

        foreach(GameObject player in playerPrefabs)
        {
            CmdResetTimer(player);
        }

    }

    [Command]
    private void CmdResetTimer(GameObject playerObj)
    {
        // resetting timer
        playerObj.GetComponent<JB_LocalPlayer>().timer = 30f;
        RpcResetTimer(playerObj);
    }

    [ClientRpc]
    private void RpcResetTimer(GameObject playerObj)
    {
        playerObj.GetComponent<JB_LocalPlayer>().timer = 30f;
    }

    [Command]
    void CmdAddResourcesToPlayer(GameObject playerObj)
    {
        //playerObj.GetComponent<JB_LocalPlayer>().currentResources += dallionsToAdd;

        int rand = Random.Range(0, 3);

        // randomisation of getting resources per turn 
        switch (rand)
        {
            case 0:
                playerObj.GetComponent<JB_LocalPlayer>().currentResources += 25;
                break;
            case 1:
                playerObj.GetComponent<JB_LocalPlayer>().currentResources += 50;
                break;
            case 2:
                playerObj.GetComponent<JB_LocalPlayer>().currentResources += 75;
                break;
            default:
                break;

        }

        // to make sure player does not exceed limit of resources
        if (playerObj.GetComponent<JB_LocalPlayer>().currentResources >= 250)
        {
            playerObj.GetComponent<JB_LocalPlayer>().currentResources = 250f;
        }

        

        RpcAddResourcesToPlayer(playerObj, playerObj.GetComponent<JB_LocalPlayer>().currentResources);

    }

    [ClientRpc]
    void RpcAddResourcesToPlayer(GameObject playerObj, float amount)
    {
        playerObj.GetComponent<JB_LocalPlayer>().currentResources = amount;
    }


}
