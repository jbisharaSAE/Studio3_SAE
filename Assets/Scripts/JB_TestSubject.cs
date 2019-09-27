using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JB_TestSubject : MonoBehaviour
{
    //private Rigidbody rb;

    //bool moveAllowed = false;

    //private void Start()
    //{
    //    rb = GetComponent<Rigidbody>();

    //}

    //private void Update()
    //{
    //    //if touch takes place
    //    if (Input.touchCount > 0)
    //    {
    //        // get the first touch
    //        Touch touch = Input.GetTouch(0);

    //        // obtain touch position, need to convert to world coordinates
    //        Vector3 touchPos = Camera.main.ScreenToWorldPoint(touch.position);

    //        // z coordinate needs to remain the same
    //        touchPos.z = 89.0f;

    //        // processing touch phases
    //        switch (touch.phase)
    //        {
    //            case TouchPhase.Began:

    //                // if player touches ship
    //                if (GetComponent<Rigidbody>() == Physics.over
    //        }
    //    }
    //}
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.GetComponent<JB_Tile>().number);
        Debug.Log(other.gameObject.GetComponent<JB_Tile>().tilePosition);
    }
    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    Debug.Log(collision.gameObject.GetComponent<JB_Tile>().number);
    //}
}
