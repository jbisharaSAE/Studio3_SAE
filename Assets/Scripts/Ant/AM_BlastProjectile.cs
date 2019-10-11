using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AM_BlastProjectile : NetworkBehaviour
{
    private float step;
    public float speed;

    private Vector3 tempTargetPos;
    public Vector3 targetTilePos;

    private Vector2 direction;

    //public AudioClip missSound;
    public AudioClip hitSound;
    public GameObject hitSpritePrefab;
    public GameObject missSpritePrefab;

    private GameObject mySprite;
    private AudioSource myAudioSource;

    private void Start()
    {
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

        step = speed * Time.deltaTime;

        transform.position = Vector2.MoveTowards(transform.position, targetTilePos, step);

        
        float distance = Vector2.Distance(transform.position, targetTilePos);

        

        if(distance <= 0.1)
        {
            Debug.Log("In distance");

            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity))
            {
                Debug.Log("Sent ray");

                // do we hit the ship
                if (hit.collider.gameObject.tag == "Square")
                {
                    Debug.Log("hit ship");

                    tempTargetPos = hit.collider.gameObject.transform.position;

                    
                    // index 0 for hitting ship
                    CmdSpawnSprite(0, tempTargetPos);
                    myAudioSource.Play();

                    // disable collider to avoid hitpoints of ship getting incorrectly calculated
                    hit.collider.gameObject.GetComponent<BoxCollider>().enabled = false;

                    hit.collider.gameObject.transform.GetComponentInParent<JB_Ship>().ShipHit();

                    Debug.Log(NetworkServer.localConnections.Count);

                }
                else if (hit.collider.gameObject.tag == "Tile")
                {
                    tempTargetPos = hit.collider.gameObject.transform.position;

                    // index 1 for missing ship
                    CmdSpawnSprite(1, tempTargetPos);

                    hit.collider.gameObject.GetComponent<BoxCollider>().enabled = false;
                    Debug.Log("hit Tile");
                    // spawn miss sprite
                }

                // do we hit a tile
                // do we hit nothing
                else
                {
                    Debug.Log("missed");
                }

                CmdDestroyGameObj(gameObject);

            }

            else
            {
                Debug.Log("Didn't hit anything");
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
