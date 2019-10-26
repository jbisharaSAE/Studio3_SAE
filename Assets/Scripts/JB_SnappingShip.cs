using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.Networking;

public class JB_SnappingShip : NetworkBehaviour
{
    // the squares that the ship are made of
    private JB_SquareSprites [] squares;

    public bool[] isTileOpen;
    private bool allTrue;

    private Vector3 lastPosition;
    private Vector3 snapPosition;
    private float staticZ;

    private void Start()
    {
        // reference to starting position of ship
        lastPosition = transform.position;

        // ensuring the z value stays the same when positioning ship
        staticZ = transform.position.z;

        // find all the scripts attached to each square
        squares = GetComponentsInChildren<JB_SquareSprites>();

        isTileOpen = new bool[squares.Length];
    }

    public void ShipPlacement()
    {
        // find all the scripts attached to each square
        squares = GetComponentsInChildren<JB_SquareSprites>();

        isTileOpen = new bool[squares.Length];
        
        for(int i = 0; i < squares.Length; ++i)
        {
            if (squares[i].isTileOpen)
            {
                isTileOpen[i] = true;
            }
            else
            {
                isTileOpen[i] = false;
            }
        }


        // if true we can place the ship there
        if (ValidPosition())
        {
            lastPosition = transform.position = new Vector3(snapPosition.x, snapPosition.y, staticZ);
            UpdatePosition(transform.position);
            // to ensure player does not place a ship on top of another ship;
            FreeOrLockShipPosition(false);
            
        }

        // if not we move it back to starting position, or it's last position on the grid
        else
        {
            // move ship to it's last legitimate position
            transform.position = lastPosition;
        }

    }

    private void UpdatePosition(Vector3 targetPos)
    {
        if (isServer)
        {
            RpcUpdatePosition(targetPos);
        }
        else
        {
            CmdUpdatePosition(targetPos);
        }
    }

    [Command]
    void CmdUpdatePosition(Vector3 targetPos)
    {
        RpcUpdatePosition(targetPos);
    }

    [ClientRpc]
    void RpcUpdatePosition(Vector3 targetPos)
    {
        transform.position = targetPos;
    }


    public bool ValidPosition()
    {
        // checks if all booleans in the array are true
        allTrue = isTileOpen.All(x => x);

        return allTrue;
    }

    private void Update()
    {
        RaycastHit hit;

        if(Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity))
        {
            if(hit.collider.gameObject.tag == "Tile")
            {
                // this snaps the position of the ship being dragged to a tile on the grid
                snapPosition = hit.collider.gameObject.transform.position;
                
            }
        }
    }

    public void FreeOrLockShipPosition(bool boolToChange)
    {

        foreach (JB_SquareSprites square in squares)
        {
            if(square.tileRef != null)
            {
                // changes boolean on tile to indicate that is now taken or open by a ship
                square.tileRef.isTileFree = boolToChange;
            }
            
        }
    }

    public void MovingShip()
    {
        // method is called when player clicks on a ship, this is so when they place it on grid, they can move it again if they change their mind

        FreeOrLockShipPosition(true);
    }

}
