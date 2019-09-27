using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JB_Tile : MonoBehaviour
{
    public int number;
    public Vector3 tilePosition;
    public RectTransform rect;
    public GameObject myCanvas;

    private void Start()
    {
        //transform.parent = myCanvas.transform;
    }
}
