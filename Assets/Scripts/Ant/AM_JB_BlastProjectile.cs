﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AM_JB_BlastProjectile : NetworkBehaviour
{
    public GameObject playerObj;

    private float step;
    public float speed;

    private Vector3 tempTargetPos;

    // target location for projetile to end
    [HideInInspector]
    public Vector3 targetTilePos;

    // vector direction to ensure projectile faces the tile
    private Vector2 direction;

    //public AudioClip missSound;
    // audio to play when player hits ship
    public AudioClip hitSound;

    // stored prefabs of missing ship and hitting ship
    public GameObject hitSpritePrefab;
    public GameObject missSpritePrefab;

    // variable used to spawn the correct sprite on the server
    private GameObject mySprite;

    // simple audio source player attached to this game object
    private AudioSource myAudioSource;

    // references to the player objects (that connect to server - currently unused - TODO)
    private GameObject[] players;

    // reference the ship type we hit
    private ShipType ship;
    
    public override void OnStartClient()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
    }

    private void Start()
    {
        if (!hasAuthority)
        {
            return;
        }

        myAudioSource = GetComponent<AudioSource>();
        myAudioSource.clip = hitSound;

        
    }

    private void FaceTile()
    {
        // maths to determine the direction of projectil to tile
        direction = new Vector2((targetTilePos.x - transform.position.x), (targetTilePos.y - transform.position.y));

        // make this projectile face the tile
        transform.up = direction;
    }

    private void Update()
    {


        FaceTile();

        if (!hasAuthority)
        {
            return;
        }

        // adjusts the speed of the projectile, making it framerate independent
        step = speed * Time.deltaTime;

        // move this projectil towards target location
        transform.position = Vector2.MoveTowards(transform.position, targetTilePos, step);

        // using distance to calculate proximity
        float distance = Vector2.Distance(transform.position, targetTilePos);


        if(distance <= 0.1)
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

    // ANTHONY'S CODE
    /*
    IEnumerator Shoot()
    {
        //Get access to abilities script
        GameObject abilityManager = GameObject.Find("EGO AbilityManager");
        Abilities abilitiesScript = abilityManager.GetComponent<Abilities>();

        for (int i = 0; i < abilitiesScript.blastProjectileGroup.Count; i++)
        {
            //Establish the target for the FIRST projectile (which is the first selectiontile)  - 1
            Vector3 target = abilitiesScript.selectionTileGroup[i].transform.position;

            //Make first projectile move to target0 position
            abilitiesScript.blastProjectileGroup[i].transform.position = Vector2.MoveTowards(abilitiesScript.blastProjectileGroup[i].transform.position, target, step);

            //Kill projectile if it touches the target
            if (abilitiesScript.blastProjectileGroup[i].transform.position == target)
            {
                HitTarget();
            }
            //Wait time before deploying another projectile
            yield return new WaitForSeconds(abilitiesScript.blastDeployDelay);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Set up speed of projectiles
        step = speed * Time.deltaTime;

        StartCoroutine("Shoot");
    }

    void HitTarget()
    {
        //Destroy the projectile when it lands
        Destroy(gameObject);
    }
    */

    // ANTHONY'S CODE
}