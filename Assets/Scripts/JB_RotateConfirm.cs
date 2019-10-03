using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class JB_RotateConfirm : NetworkBehaviour
{
    public static GameObject shipObj;
    public JB_GameManager gameManagerScript;
    public GameObject errorAlertTextObj;
    public int placementIndex = 0;

    private void Start()
    {
        
    }

    public void RotateShip()
    {
        // if shipObj is not empty, rotate 90 degrees
        if(shipObj != null)
        {
            shipObj.transform.Rotate(0f, 0f, 90f);
        }


    }

    public void FindGameManagerObj()
    {
        // find the game manager gameobject in scene
        //gameManagerScript = GameObject.FindGameObjectWithTag("GameManager").GetComponent<JB_GameManager>();
    }

    public void ConfirmPosition()
    {
        if(shipObj != null)
        {
            shipObj.SendMessage("LockShipPosition");

            if (shipObj.GetComponent<JB_SnappingShip>().ValidPosition()) // double checks that ship is valid position on the grid 
            {
                // if true we can lock the ship's position
                shipObj.GetComponent<DragObject>().canDrag = false;

                // index counter unique to each player to determine when game starts
                ++placementIndex;

                // checks if player has all ships in position and is ready to play
                gameManagerScript.CmdCheckPlayerReady(placementIndex);

                

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
        errorAlertTextObj.SetActive(true);
        yield return new WaitForSeconds(4f);
        errorAlertTextObj.SetActive(false);
    }
}
