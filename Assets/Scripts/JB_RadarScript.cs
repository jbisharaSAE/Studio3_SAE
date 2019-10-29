using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class JB_RadarScript : NetworkBehaviour
{
    // speed of expanding collider
    private float speed;
    // when i want my collider to stop expanding
    private bool expand = true;
    // position of ship square
    //private Vector3 squarePos;

    // reference to owner's player's obj
    public GameObject playerObj;

    // reference to enemy grid layout
    private GameObject gridManagerObj;

    public AudioClip radarSound;
    private AudioSource myAudioSource;

    public GameObject radarDetectPrefab;
    private GameObject radarDetect;

    [SyncVar]
    private int x; // x position of tile
    [SyncVar]
    private int y; // y position of tile

    public override void OnStartAuthority()
    {
        CmdPlayRadarSound();
    }

    [Command]
    void CmdPlayRadarSound()
    {
        myAudioSource.clip = radarSound;
        myAudioSource.Play();
        RpcPlayRadarSound();
    }


    [ClientRpc]
    void RpcPlayRadarSound()
    {
        myAudioSource.clip = radarSound;
        myAudioSource.Play();
    }

    private void Awake()
    {
        myAudioSource = GetComponent<AudioSource>();
    }


    // Update is called once per frame
    void Update()
    {
        
        if (expand)
        {
            speed += 0.02f;
            transform.localScale += new Vector3(speed, speed, 0);
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        expand = false;
        Debug.Log("testing trigger");
        if (other.gameObject.tag == "Square")
        {
            //squarePos = other.gameObject.transform.position;
            x = other.gameObject.GetComponent<JB_SquareSprites>().x;
            y = other.gameObject.GetComponent<JB_SquareSprites>().y;
            gridManagerObj = other.gameObject.GetComponent<JB_SquareSprites>().gridManagerObj;

            playerObj.GetComponent<JB_LocalPlayer>().ShipDetection(gridManagerObj, x, y);

            expand = false;

            GetComponent<SphereCollider>().enabled = false;

            CmdDestroyThisRadar(gameObject);

            CmdSpawnRadarDetect(other.gameObject.transform.position);

        }
    }


    [Command]
    void CmdSpawnRadarDetect(Vector3 targetPos)
    {
        radarDetect = Instantiate(radarDetectPrefab, targetPos, Quaternion.identity);
        NetworkServer.Spawn(radarDetect);

        RpcSpawnRadarDetect(radarDetect);

    }

    [ClientRpc]
    void RpcSpawnRadarDetect(GameObject radarObj)
    {
        radarDetect = radarObj;
    }
    

    [Command]
    void CmdDestroyThisRadar(GameObject obj)
    {
        expand = false;
        Destroy(obj, 3);
        RpcDestroyThisRadar(obj);
    }
    [ClientRpc]
    void RpcDestroyThisRadar(GameObject obj)
    {
        expand = false;
        Destroy(obj, 3);
    }
}
