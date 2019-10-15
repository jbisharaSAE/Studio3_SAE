using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class JB_BarrageProjectile : NetworkBehaviour
{
    private float step;

    public GameObject playerObj;

    public GameObject hitSpritePrefab;
    public GameObject missSpritePrefab;

    private GameObject mySprite;

    // speed of projectile
    public float speed;

    // location for projectile to travel to
    public Vector3 targetPos;

    // delay for each projectile
    public float delayTime;

    // timer
    private float time;

    // reference the ship type we hit
    private ShipType ship;

    private Vector3 tempTargetPos;

    // Update is called once per frame
    void Update()
    {
        step = speed * Time.deltaTime;

        if (delayTime > time)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPos, step);
        }
        else
        {
            time += Time.deltaTime; 
        }
        

        float distance = Vector2.Distance(transform.position, targetPos);

        if(distance < 0.1f)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity))
            {
                Debug.Log("Sent ray");

                // do we hit the ship
                if (hit.collider.gameObject.tag == "Square")
                {
                    Debug.Log("hit ship");

                    // take tile position from click and store in our variable
                    tempTargetPos = hit.collider.gameObject.transform.position;

                    Transform hitPos = hit.collider.gameObject.transform;

                    // index 0 for hitting ship
                    CmdSpawnSprite(0, tempTargetPos);

                    //myAudioSource.Play();


                    // disable collider to avoid hitpoints of ship getting incorrectly calculated
                    hit.collider.gameObject.GetComponent<BoxCollider>().enabled = false;

                    // getting reference to the parent object (the ship)
                    GameObject shipObj = hit.collider.gameObject.transform.parent.gameObject;

                    ship = shipObj.GetComponent<JB_Ship>().shipType;

                    // calling function to count ship hits
                    playerObj.GetComponent<JB_LocalPlayer>().FindShipHit(ship, shipObj, hitPos.position);

                    CmdDestroyGameObj(gameObject);
                    return;
                }
                else if (hit.collider.gameObject.tag == "Tile")
                {
                    tempTargetPos = hit.collider.gameObject.transform.position;

                    // index 1 for missing ship
                    CmdSpawnSprite(1, tempTargetPos);

                    hit.collider.gameObject.GetComponent<BoxCollider>().enabled = false;
                    Debug.Log("hit Tile");
                    // spawn miss sprite
                    CmdDestroyGameObj(gameObject);
                    return;
                }

                // do we hit a tile
                // do we hit nothing
                else
                {
                    Debug.Log("missed");
                    CmdDestroyGameObj(gameObject);
                    return;
                }



            }

            else
            {
                CmdDestroyGameObj(gameObject);
                Debug.Log("Didn't hit anything");
                return;
            }
            // spawn particle effects
            // ray cast for ship hit or miss
        }
    }

   
    [Command]
    void CmdDestroyGameObj(GameObject gameObj)
    {
        GameObject newObj = gameObj;
        RpcDestroyGameObj(newObj);
        Destroy(gameObj);
    }

    [ClientRpc]
    void RpcDestroyGameObj(GameObject gameObj)
    {
        Destroy(gameObj);
    }

    [Command]
    void CmdSpawnSprite(int index, Vector3 targetPos)
    {
        switch (index)
        {
            case 0:
                mySprite = Instantiate(hitSpritePrefab, targetPos, Quaternion.identity);
                break;
            case 1:
                mySprite = Instantiate(missSpritePrefab, targetPos, Quaternion.identity);
                break;
            default:
                break;
        }

        GameObject newSprite = mySprite;

        NetworkServer.Spawn(mySprite);

        //RpcSpawnSprite(mySprite);

    }

    [ClientRpc]
    void RpcSpawnSprite(GameObject spriteObj)
    {
        mySprite = spriteObj;
    }
}
