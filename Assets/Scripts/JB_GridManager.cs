using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JB_GridManager : MonoBehaviour
{
    [SerializeField]
    private float size = 1f;


    //    public JB_Tile[] tileScript;
    //    private GameObject[] tiles;
    //    private RectTransform[] childTransforms;

    public GameObject tile;
    
    [SerializeField]
    private float length = 8.77f;
    [SerializeField]
    private float width = 8.77f;

    private int index = 0;
    
    //private Vector3 newPosLength;
    //private Vector3 newPosWidth;

    public Vector3 GetNearestPointOnGrid(Vector3 position)
    {
        // offset calculation, if we need to move the grid
        position -= transform.position;

        int xCount = Mathf.RoundToInt(position.x / size);
        int yCount = Mathf.RoundToInt(position.y / size);
        int zCount = Mathf.RoundToInt(position.z / size);

        Vector3 result = new Vector3((float)xCount*size, (float)yCount*size, (float)zCount*size);

        result += transform.position;

        return result;
    }

    // Start is called before the first frame update
    void Start()
    {

        for(int x = 0; x < 12; ++x)
        {
            length += 9f;

            width = 9f; // resetting value to it's original number

            for(int y = 0; y < 12; ++y) 
            {
                // spawn grid cell and initialising variables
                Vector3 spawnPoint = new Vector3(transform.position.x + length, transform.position.y + width, 89f);
                GameObject newTile = Instantiate(tile, spawnPoint, Quaternion.identity);
                newTile.GetComponent<JB_Tile>().tilePosition = newTile.transform.position;
                newTile.GetComponent<JB_Tile>().number = index;

                ++index;
                width += 9f;
            }
        }


        //tiles = new GameObject[12, 12];

        //// laying out grid of tiles
        //for(int i =0; i < 12; ++i)
        //{
        //    //newPosLength = new Vector3(transform.position.x + length, transform.position.y, transform.position.z);

        //    //tiles[i]Instantiate(tile, newPosLength, Quaternion.identity);

        //    length += length;
        //    width = 8.7f;
        //    for(int j = 0; j < 12; ++j)
        //    {
        //        newPosWidth = new Vector3(transform.position.x + length, transform.position.y - width, transform.position.z);

        //        Instantiate(tile, newPosWidth, Quaternion.identity);

        //        width += width;
        //    }
        //}
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
