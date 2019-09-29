using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class JB_LocalPlayer : NetworkBehaviour
{
    public GameObject[] ships;

    public int playerID;
    // Start is called before the first frame update
    void Start()
    {

        
        
        //if local player, enable ship, otherwise turn them off
        if (this.isLocalPlayer)
        {
            // confirmed test, this code works
            //this.OnSetLocalVisibility(true);
            //foreach (GameObject ship in ships)
            //{
            //    ship.SetActive(true);
            //}

        }
        else
        {
            this.OnSetLocalVisibility(false);
            //foreach (GameObject ship in ships)
            //{
            //    ship.SetActive(false);
            //}
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
