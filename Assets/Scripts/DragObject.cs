using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragObject : MonoBehaviour
{
    private Vector3 mOffset;
    private float mZCoord;
    private JB_SnappingShip snapShipScript;
    private bool isDragging;

    private void Start()
    {
        snapShipScript = GetComponent<JB_SnappingShip>();
    }

    void OnMouseDown()
    {
        mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;

        // Store offset = gameobject world pos - mouse world pos
        mOffset = gameObject.transform.position - GetMouseAsWorldPoint();

        Debug.Log("clicked");
    }

    private Vector3 GetMouseAsWorldPoint()
    {
        // Pixel coordinates of mouse (x,y)
        Vector3 mousePoint = Input.mousePosition;

        // z coordinate of game object on screen
        mousePoint.z = mZCoord;

        // Convert it to world points
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    void OnMouseDrag()
    {
        isDragging = true;
        transform.position = GetMouseAsWorldPoint() + mOffset;
        
    }

    private void OnMouseUp()
    {
        snapShipScript.SendMessage("ShipPlacement");
        Debug.Log("testing OnMouseUp");
    }

    private void OnMouseExit()
    {
        // call method to snap ship to the grid, script should be attached to this game object
        //snapShipScript.SendMessage("ShipPlacement");
        //Debug.Log("testing OnMouseExit");
    }
}