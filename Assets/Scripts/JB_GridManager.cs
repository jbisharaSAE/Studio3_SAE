using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JB_GridManager : MonoBehaviour
{
    public JB_Tile[] tileScript;

    // Start is called before the first frame update
    void Start()
    {
        tileScript = GetComponentsInChildren<JB_Tile>();

        for(int i = 0; i < tileScript.Length; ++i)
        {
            tileScript[i].number = i;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
