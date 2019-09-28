using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JB_SquareSprites : MonoBehaviour
{
    public bool isTileOpen;

    private void Update()
    {
        RaycastHit hit;

        if(Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity))
        {
            if(hit.collider.gameObject.tag == "Tile")
            {
                isTileOpen = hit.collider.gameObject.GetComponent<JB_Tile>().isTileFree;
            }
            
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
