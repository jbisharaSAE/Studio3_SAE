using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JB_TestSubject : MonoBehaviour
{
    private void Update()
    {
        RaycastHit hit;

        if(Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity))
        {
            if(hit.collider.gameObject.tag == "Tile")
            {
                Debug.Log(hit.collider.gameObject.GetComponent<JB_Tile>().number);
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
