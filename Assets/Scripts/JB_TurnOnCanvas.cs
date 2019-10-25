using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class JB_TurnOnCanvas : NetworkManager
{
    public GameObject myCanvas;
    // Start is called before the first frame update
    void Start()
    {
        myCanvas.SetActive(true);
    }

    
}
