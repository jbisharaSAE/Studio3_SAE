using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JB_GridManager : MonoBehaviour
{
    //    public JB_Tile[] tileScript;
    //    private GameObject[] tiles;
    //    private RectTransform[] childTransforms;

    public GameObject tile;
    private float length = 8.7f;
    private float width = 8.7f;
    private Vector3 newPos;

    // Start is called before the first frame update
    void Start()
    {
        // laying out grid of tiles
        for(int i =0; i < 12; ++i)
        {
            newPos = new Vector3(tile.transform.position.x + length, tile.transform.position.y, tile.transform.position.z);

            Instantiate(tile, newPos, Quaternion.identity);

            length += length;

            for(int j = 0; j < 12; ++j)
            {
                newPos = new Vector3(tile.transform.position.x, tile.transform.position.y + width, tile.transform.position.z);

                Instantiate(tile, newPos, Quaternion.identity);

                width += width;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
