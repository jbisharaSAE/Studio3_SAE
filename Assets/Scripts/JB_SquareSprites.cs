using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JB_SquareSprites : MonoBehaviour
{
    public bool isTileOpen;
    public JB_Tile tileRef;

    private void Update()
    {
        RaycastHit hit;

        if(Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity))
        {
            if(hit.collider.gameObject.tag == "Tile")
            {
                // grabs boolean on tile to ensure if it is free or not
                isTileOpen = hit.collider.gameObject.GetComponent<JB_Tile>().isTileFree;

                //reference to the tile to be considered taken after player confirms ship position
                tileRef = hit.collider.gameObject.GetComponent<JB_Tile>();
            }
            else
            {
                // if raycast hits an object that's not a tile, that means ship is not in viable position
                isTileOpen = false;
            }
            
        }
        else
        {
            // if raycast hits no game objects, ship is not in viable position
            isTileOpen = false;
        }
            
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.gameObject.GetComponent<JB_Tile>().number);
        Debug.Log(other.gameObject.GetComponent<JB_Tile>().tilePosition);
    }
    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    Debug.Log(collision.gameObject.GetComponent<JB_Tile>().number);
    //}
}
