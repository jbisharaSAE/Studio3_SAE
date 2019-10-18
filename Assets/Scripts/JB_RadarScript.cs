using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class JB_RadarScript : NetworkBehaviour
{
    // speed of expanding collider
    private float speed;
    // when i want my collider to stop expanding
    private bool expand = true;
    // position of ship square
    private Vector3 squarePos;

    // reference to owner's player's obj
    public GameObject playerObj;

    // reference to enemy grid layout
    private GameObject gridManagerObj;

    [SyncVar]
    private int x; // x position of tile
    [SyncVar]
    private int y; // y position of tile

    // Update is called once per frame
    void Update()
    {
        

        if (expand)
        {
            speed += 0.02f;
            transform.localScale += new Vector3(speed, speed, 0);
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("testing trigger");
        if (other.gameObject.tag == "Square")
        {
            squarePos = other.gameObject.transform.position;
            x = other.gameObject.GetComponent<JB_SquareSprites>().x;
            y = other.gameObject.GetComponent<JB_SquareSprites>().y;
            gridManagerObj = other.gameObject.GetComponent<JB_SquareSprites>().gridManagerObj;

            playerObj.GetComponent<JB_LocalPlayer>().ShipDetection(gridManagerObj, x, y);

            expand = false;

            Destroy(gameObject, 0.1f);

        }
    }


}
