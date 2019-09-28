using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DragObject : NetworkBehaviour
{
    private Vector3 offset;
    private float zCoord;
    private JB_SnappingShip snapShipScript;
    private bool isDragging;

    private void Start()
    {

        snapShipScript = GetComponent<JB_SnappingShip>();
    }

    void OnMouseDown()
    {
        if (this.isLocalPlayer)
        {
            zCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;

            // Store offset = gameobject world pos - mouse world pos
            offset = gameObject.transform.position - GetMouseAsWorldPoint();

            // change ship reference to rotate
            JB_RotateConfirm.shipObj = gameObject;
            
        }
        
    }

    private Vector3 GetMouseAsWorldPoint()
    {
        if (this.isLocalPlayer)
        {
            // Pixel coordinates of mouse (x,y)
            Vector3 mousePoint = Input.mousePosition;

            // z coordinate of game object on screen
            mousePoint.z = zCoord;

            // Convert it to world points
            return Camera.main.ScreenToWorldPoint(mousePoint);
        }
        
    }

    void OnMouseDrag()
    {
        if (this.isLocalPlayer)
        {
            transform.position = GetMouseAsWorldPoint() + offset;
        }
        
        
    }

    private void OnMouseUp()
    {
        if (this.isLocalPlayer)
        {
            // when mouse (or touch) is lifted from ship, snap the ship to the grid position closest to it
            snapShipScript.SendMessage("ShipPlacement");
        }
        
    }

  
}