using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JB_Ship : MonoBehaviour

{
    public ShipType shipType;

    public int shipHealth;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShipHit()
    {
        Debug.Log("Ship Hit!!!! ++ = " + shipHealth);
        --shipHealth;
    }
}
