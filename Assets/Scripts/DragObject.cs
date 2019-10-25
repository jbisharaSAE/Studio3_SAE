using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DragObject : NetworkBehaviour
{
    private Vector3 offset;
    private float zCoord;
    private JB_SnappingShip snapShipScript;

    private GameObject[] ships;

    // used to determine which player this ship belongs to
    [SyncVar]
    public int playerID; 

    public bool canDrag = true;
    
    public GameObject shipSprite;

  

    public override void OnStartAuthority()
    {
        

        if (hasAuthority == false)
        {
            return;
        }
    }

    private void Start()
    {
        snapShipScript = GetComponent<JB_SnappingShip>();
    }

    public void EnableShipSprite()
    {
        if (!hasAuthority)
        {
            return;
        }
        shipSprite.SetActive(true);
    }

    void OnMouseDown()
    {
        if (GetComponent<NetworkIdentity>().hasAuthority == false)
        {
            return;
        }

        if (canDrag)
        {
            zCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;

            // Store offset = gameobject world pos - mouse world pos
            offset = gameObject.transform.position - GetMouseAsWorldPoint();

            // change ship reference to rotate
            JB_LocalPlayer.shipObj = gameObject;


            // frees up tiles that were taken up by ship's last position
            snapShipScript.SendMessage("MovingShip");
            Debug.Log("Clicked, shipObj variable: "); // + JB_LocalPlayer.shipObj);
        }

        //RemoveSpriteEnemyShips();

    }

    private Vector3 GetMouseAsWorldPoint()
    {
        if (GetComponent<NetworkIdentity>().hasAuthority == false)
        {
            return Vector3.zero;
        }

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
        if (GetComponent<NetworkIdentity>().hasAuthority == false)
        {
            return;
        }

        if (canDrag)
        {
            transform.position = GetMouseAsWorldPoint() + offset;
        }
        

    }

    private void OnMouseUp()
    {
        if (GetComponent<NetworkIdentity>().hasAuthority == false)
        {
            return;
        }
        
        if (canDrag)
        {
            // when mouse (or touch) is lifted from ship, snap the ship to the grid position closest to it
            snapShipScript.SendMessage("ShipPlacement");
        }
        
    }



    //void RemoveSpriteEnemyShips()
    //{
    //    if (!runOnce)
    //    {
    //        GameObject[] allShips = GameObject.FindGameObjectsWithTag("Ship");

    //        foreach (GameObject ship in allShips)
    //        {
    //            if (playerID != ship.GetComponent<DragObject>().playerID)
    //            {
    //                ship.GetComponent<DragObject>().shipSprite.SetActive(false);
    //            }

    //        }
    //        runOnce = true;
    //    }

    //}


}