using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class JB_VolleyProjectile : NetworkBehaviour
{
    private float step;

    public GameObject playerObj;

    public GameObject hitSpritePrefab;
    public GameObject missSpritePrefab;

    private GameObject mySprite;

    public AudioClip barrageLaunchSound;
    public AudioClip hitShipSound;
    public AudioClip shieldOffSound;

    private AudioSource myAudioSource;

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

    private void Start()
    {
        Destroy(gameObject, 8f);
    }

    public override void OnStartAuthority()
    {
        CmdLaunchProjectile();
    }

    [Command]
    void CmdLaunchProjectile()
    {
        myAudioSource.clip = barrageLaunchSound;
        myAudioSource.Play();
        RpcLaunchProjectile();
    }

    [ClientRpc]
    void RpcLaunchProjectile()
    {
        myAudioSource.clip = barrageLaunchSound;
        myAudioSource.Play();
    }

    private void Awake()
    {
        myAudioSource = GetComponent<AudioSource>();

    }


    // Update is called once per frame
    void Update()
    {
        step = speed * Time.deltaTime;

        if (delayTime < time)
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
                    Debug.Log("hit ship " + hit.collider.gameObject.name);

                    // take tile position from click and store in our variable
                    tempTargetPos = hit.collider.gameObject.transform.position;

                    Transform hitPos = hit.collider.gameObject.transform;

                    // index 0 for hitting ship
                    CmdSpawnSprite(0, tempTargetPos);

                    CmdShipHitAudio();


                    // disable collider to avoid hitpoints of ship getting incorrectly calculated
                    hit.collider.gameObject.GetComponent<BoxCollider>().enabled = false;

                    hit.collider.gameObject.GetComponent<JB_SquareSprites>().tileRef.GetComponent<BoxCollider>().enabled = false;

                    // getting reference to the parent object (the ship)
                    GameObject shipObj = hit.collider.gameObject.transform.parent.gameObject;

                    ship = shipObj.GetComponent<JB_Ship>().shipType;

                    // calling function to count ship hits
                    playerObj.GetComponent<JB_LocalPlayer>().FindShipHit(ship, shipObj, hitPos.position);

                    DestroyGameObject(gameObject);


                    return;
                }
                else if (hit.collider.gameObject.tag == "Tile")
                {
                    tempTargetPos = hit.collider.gameObject.transform.position;

                    // index 1 for missing ship
                    CmdSpawnSprite(1, tempTargetPos);

                    // so player does not aim at the same tile twice
                    hit.collider.gameObject.GetComponent<BoxCollider>().enabled = false;

                    hit.collider.gameObject.GetComponent<BoxCollider>().enabled = false;
                    Debug.Log("hit Tile " + hit.collider.gameObject.name);
                    
                    
                    return;
                }

                else if (hit.collider.gameObject.tag == "Shield")
                {
                    Debug.Log("hit shield " + hit.collider.name);
                    //hit.collider.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                    //hit.collider.gameObject.GetComponent<BoxCollider>().enabled = false;
                    CmdShieldOff();
                    //Destroy(hit.collider.gameObject);
                    DestroyGameObject(hit.collider.gameObject.transform.parent.gameObject);
                    DestroyGameObject(gameObject);
                }

            }
            

            
            // spawn particle effects
            // ray cast for ship hit or miss
        }
    }


    void DestroyGameObject(GameObject gameObj)
    {
        if (isServer)
        {
            RpcDestroyGameObj(gameObj);
        }
        else
        {
            CmdDestroyGameObj(gameObj);
        }
    }

    [Command]
    void CmdDestroyGameObj(GameObject gameObj)
    {
        //GameObject newObj = gameObj;
        RpcDestroyGameObj(gameObj);
        //Destroy(gameObj);
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

        CmdDestroyGameObj(gameObject);

    }

    [Command]
    void CmdShipHitAudio()
    {
        //myAudioSource.clip = hitShipSound;
        //myAudioSource.Play();
        myAudioSource.PlayOneShot(hitShipSound);
        RpcShipHitAudio();
    }

    [ClientRpc]
    void RpcShipHitAudio()
    {
        //myAudioSource.clip = hitShipSound;
        //myAudioSource.Play();
        myAudioSource.PlayOneShot(hitShipSound);
    }

    [Command]
    void CmdShieldOff()
    {
        //myAudioSource.clip = shieldOffSound;
        //myAudioSource.Play();
        myAudioSource.PlayOneShot(shieldOffSound);
        RpcShieldOff();
    }

    [ClientRpc]
    void RpcShieldOff()
    {
        //myAudioSource.clip = shieldOffSound;
        //myAudioSource.Play();
        myAudioSource.PlayOneShot(shieldOffSound);
    }

}
