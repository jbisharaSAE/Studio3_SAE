using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class JB_SnappingShip : MonoBehaviour
{
    // the squares that the ship are made of
    private JB_SquareSprites [] squares;
    private bool[] isTileOccupied;
    private bool allTrue;

    private Vector3 snapPosition;
    private float staticZ;

    private void Start()
    {
        // ensuring the z value stays the same when positioning ship
        staticZ = transform.position.z;
    }

    public void ShipPlacement()
    {
        // find all the scripts attached to each square
        squares = GetComponentsInChildren<JB_SquareSprites>();

        isTileOccupied = new bool[squares.Length];
        
        for(int i = 0; i < squares.Length; ++i)
        {
            if (squares[i].isTileOpen)
            {
                isTileOccupied[i] = true;
            }
            else
            {
                isTileOccupied[i] = false;
            }
        }

        allTrue = isTileOccupied.All(x => x);

        if (allTrue)
        {
            transform.position = new Vector3(snapPosition.x, snapPosition.y, staticZ);
        }

    }

    private void Update()
    {
        RaycastHit hit;

        if(Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity))
        {
            if(hit.collider.gameObject.tag == "Tile")
            {
                snapPosition = hit.collider.gameObject.transform.position;
                Debug.Log(hit.collider.gameObject.transform.position);
            }
        }
    }

}
