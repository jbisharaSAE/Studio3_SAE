using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JB_RotateConfirm : MonoBehaviour
{
    public static GameObject shipObj;

    public void RotateShip()
    {
        shipObj.transform.Rotate(0f, 0f, 90f);

    }

    public void ConfirmPosition()
    {
        shipObj.SendMessage("LockShipPosition");
    }
}
