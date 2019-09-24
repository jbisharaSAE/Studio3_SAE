using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MovePlayer : NetworkBehaviour
{

    [SerializeField]
    private float speed;
    private Vector2 mousePos;

    private Vector3 mOffset;
    private float mZCoord;

    public GameObject testSubject;

    private void Start()
    {
        testSubject = GameObject.Find("TestSubject");
    }
    void FixedUpdate()
    {
        if (this.isLocalPlayer)
        {
            float movement = Input.GetAxis("Horizontal");
            //GetComponent<Rigidbody2D>().velocity = new Vector2(movement * speed, 0.0f);
        }
    }

    private void Update()
    {
        if (this.isLocalPlayer)
        {
            mousePos = Input.mousePosition;

            mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;

            // Store offset = gameobject world pos - mouse world pos
            mOffset = gameObject.transform.position - GetMouseAsWorldPoint();

            //testSubject.transform.position = GetMouseAsWorldPoint();
        }
    }




    private Vector3 GetMouseAsWorldPoint()
    {
        // Pixel coordinates of mouse (x,y)
        Vector3 mousePoint = Input.mousePosition;

        // z coordinate of game object on screen
        mousePoint.z = 89;

        // Convert it to world points
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.GetComponent<JB_Tile>().number);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.GetComponent<JB_Tile>().number);
    }

}