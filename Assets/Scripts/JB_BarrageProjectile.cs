using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class JB_BarrageProjectile : NetworkBehaviour
{
    public GameObject playerObj;

    private float step;
    public float speed;

    private Vector3 tempTargetPos;

    // target location for projetile to end
    [HideInInspector]
    public Vector3 targetPos;

    // vector direction to ensure projectile faces the tile
    private Vector2 direction;

    // audio to play when projectile launches
    public AudioClip barrageAudio;
    // audio to play when player hits ship
    public AudioClip hitShipSound;
    public AudioClip shieldOffSound;

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

    private float timer;
    private float time;

    private Rigidbody2D rb;
    private bool runOnce = true;

    public float rotateSpeed = 200f;
    public float delayTime;


    [Command]
    void CmdLaunchProjectile()
    {
        myAudioSource.clip = barrageAudio;
        myAudioSource.Play();
        RpcLaunchProjectile();
    }

    [ClientRpc]
    void RpcLaunchProjectile()
    {
        myAudioSource.clip = barrageAudio;
        myAudioSource.Play();
    }

    private void Awake()
    {
        myAudioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();

    }

   
    private void FixedUpdate()
    {
        Vector2 direction = (Vector2)targetPos - rb.position;

        direction.Normalize();

        float rotAmount = Vector3.Cross(direction, transform.up).z;

        rb.velocity = transform.up * speed;

        //transform.Translate(transform.up * speed);

        if(delayTime < time)
        {
            if (runOnce)
            {
                CmdLaunchProjectile();
                runOnce = false;
            }

            if (timer > 0.5f)
            {
                rb.angularVelocity = -rotAmount * rotateSpeed;
            }
            else
            {
                timer += Time.deltaTime;

            }
        }
        else
        {
            time += Time.deltaTime;
        }
        

        // using distance to calculate proximity
        float distance = Vector2.Distance(transform.position, targetPos);


        if (distance <= 3f)
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

                    // disable collider to avoid hitpoints of ship getting incorrectly calculated
                    hit.collider.gameObject.GetComponent<BoxCollider>().enabled = false;

                    // getting reference to the parent object (the ship)
                    GameObject shipObj = hit.collider.gameObject.transform.parent.gameObject;

                    ship = shipObj.GetComponent<JB_Ship>().shipType;

                    CmdShipHitAudio();

                    // calling function to count ship hits
                    playerObj.GetComponent<JB_LocalPlayer>().FindShipHit(ship, shipObj, hitPos.position);

                    CmdDestroyGameObj(gameObject);
                    return;
                }
                // do we hit a tile
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


                // do we hit shield
                else if (hit.collider.gameObject.tag == "Shield")
                {
                    CmdShieldOff();
                    Destroy(hit.collider.gameObject);
                    CmdDestroyGameObj(gameObject);
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

    private void Update()
    {
        // adjusts the speed of the projectile, making it framerate independent
        //step = speed * Time.deltaTime;

       
        //FaceTile();

        if (!hasAuthority)
        {
            return;
        }

        
        // move this projectile towards target location
        //transform.position = Vector2.MoveTowards(transform.position, targetTilePos, step);

       
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

        NetworkServer.Spawn(mySprite);



    }

    [Command]
    void CmdShipHitAudio()
    {
        myAudioSource.clip = hitShipSound;
        myAudioSource.Play();
        RpcShipHitAudio();
    }

    [ClientRpc]
    void RpcShipHitAudio()
    {
        myAudioSource.clip = hitShipSound;
        myAudioSource.Play();
    }


    [Command]
    void CmdShieldOff()
    {
        myAudioSource.clip = shieldOffSound;
        myAudioSource.Play();
        RpcShieldOff();
    }

    [ClientRpc]
    void RpcShieldOff()
    {
        myAudioSource.clip = shieldOffSound;
        myAudioSource.Play();
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
