using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class JB_GridManager : MonoBehaviour
{

    public GameObject tile;
    
    [SerializeField]
    private float length = 8.77f;
    [SerializeField]
    private float width = 8.77f;

    private int index = 0;

    //private Vector3 newPosLength;
    //private Vector3 newPosWidth;



    // Start is called before the first frame update
    void Start()
    {

        for (int x = 0; x < 12; ++x)
        {
            length += 9f;

            width = 9f; // resetting value to it's original number

            for (int y = 0; y < 12; ++y)
            {
                // spawn grid cell and initialising variables
                Vector3 spawnPoint = new Vector3(transform.position.x + length, transform.position.y + width, 89f);
                GameObject newTile = Instantiate(tile, spawnPoint, Quaternion.identity);
                newTile.transform.parent = gameObject.transform;
                
                // assigning variables on each tiles
                newTile.GetComponent<JB_Tile>().tilePosition = newTile.transform.position;
                newTile.GetComponent<JB_Tile>().number = index;

                ++index;
                width += 9f;
            }
        }


    }

    
}
