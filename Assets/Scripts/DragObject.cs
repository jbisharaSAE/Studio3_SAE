using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Networking;

public class DragObject : MonoBehaviour
{
    private Vector3 offset;
    private float zCoord;
    private JB_SnappingShip snapShipScript;
    public bool canDrag = true;

    private void Start()
    {
        
        
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
            JB_GameManager.shipObj = gameObject;

            // frees up tiles that were taken up by ship's last position
            snapShipScript.SendMessage("MovingShip");
            Debug.Log("Clicked");
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