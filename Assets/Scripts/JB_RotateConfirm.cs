using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JB_RotateConfirm : MonoBehaviour
{
    public static GameObject shipObj;
    public JB_GameManager gameManagerScript;
    public GameObject errorAlertTextObj;
    public int placementIndex = 0;

    public void RotateShip()
    {
        if(shipObj != null)
        {
            shipObj.transform.Rotate(0f, 0f, 90f);
        }


    }

    public void ConfirmPosition()
    {
        if(shipObj != null)
        {
            shipObj.SendMessage("LockShipPosition");

            if (shipObj.GetComponent<JB_SnappingShip>().ValidPosition())
            {
                // double checks that ship is valid position on the grid, if true we can lock the ship's position
                shipObj.GetComponent<DragObject>().canDrag = false;

                // index counter to determine when game starts
                ++placementIndex;

                // checks if player has all ships in position and is ready to play
                gameManagerScript.CheckPlayerReady(placementIndex);
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
