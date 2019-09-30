using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JB_RotateConfirm : MonoBehaviour
{
    public static GameObject shipObj;

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
            }
            else
            {
                // if not, the ship can still be moved by player
                shipObj.GetComponent<DragObject>().canDrag = true;
            }
        }
        
        
    }
}
