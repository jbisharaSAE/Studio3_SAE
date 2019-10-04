using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DragObject : NetworkBehaviour
{
    private Vector3 offset;
    private float zCoord;
    private JB_SnappingShip snapShipScript;
    public bool canDrag = true;

    public override void OnStartAuthority()
    {
        Debug.Log(GetComponent<NetworkIdentity>().hasAuthority);

        if (GetComponent<NetworkIdentity>().hasAuthority == false)
        {
            return;

        }

       
    }

    private void Start()
    {
        SpriteRenderer[] squareSprites = GetComponentsInChildren<SpriteRenderer>();

        // for some unknown reason, the sprites were being disabled, I am forcing them to enabled
        foreach (SpriteRenderer square in squareSprites)
        {
            square.GetComponent<SpriteRenderer>().enabled = true;
        }


        snapShipScript = GetComponent<JB_SnappingShip>();
    }

    void OnMouseDown()
    {
        if (canDrag)
        {
            zCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;

            // Store offset = gameobject world pos - mouse world pos
            offset = gameObject.transform.position - GetMouseAsWorldPoint();

            // change ship reference to rotate
            JB_LocalPlayer.shipObj = gameObject;
            

            // frees up tiles that were taken up by ship's last position
            snapShipScript.SendMessage("MovingShip");
            Debug.Log("Clicked, shipObj variable: " + JB_LocalPlayer.shipObj);
        }
        
    }

    private Vector3 GetMouseAsWorldPoint()
    {
        if (canDrag)
        {
            // Pixel coordinates of mouse (x,y)
            Vector3 mousePoint = Input.mousePosition;

            // z coordinate of game object on screen
            mousePoint.z = zCoord;

            // Convert it to world points
            return Camera.main.ScreenToWorldPoint(mousePoint);
        }

        else
        {
            return Vector3.zero;
        }
        

    }

    void OnMouseDrag()
    {
        if (canDrag)
        {
            transform.position = GetMouseAsWorldPoint() + offset;
        }
        

    }

    private void OnMouseUp()
    {
        if (canDrag)
        {
            // when mouse (or touch) is lifted from ship, snap the ship to the grid position closest to it
            snapShipScript.SendMessage("ShipPlacement");
        }
        
    }

  
}