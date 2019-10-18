using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class JB_Tile : NetworkBehaviour
{
    public int number;

    [SyncVar]
    public Vector3 tilePosition;
    public bool isTileFree = true;

    public int visited = 1;
    public int x = 0;
    public int y = 0;

 
}
