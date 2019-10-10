using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BlastProjectile : NetworkBehaviour
{
    private float step;
    public float speed;

    public Vector3 targetTilePos;

    

    private void Update()
    {
        step = speed * Time.deltaTime;

        transform.position = Vector3.MoveTowards(transform.position, targetTilePos, step);

        float distance = Vector3.Distance(transform.position, targetTilePos);

        if(distance <= 0.1)
        {
            Debug.Log("In distance");

            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.position + new Vector3(0, 0, 1), out hit))
            {
                Debug.Log("Sent ray");
                // do we hit the ship
                if (hit.transform.tag == "Ship")
                {
                    Debug.Log("hit ship");
                    AudioManager audioManagerScript = GameObject.Find("Audio Manager").GetComponent<AudioManager>();
                    audioManagerScript.PlayBlast();
                }

                // do we hit a tile
                // do we hit nothing
                else
                {
                    Debug.Log("missed");
                }

                //Destroy(gameObject);

            }

            else
            {
                Debug.Log("Didn't hit anything");
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, 0, 1));
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
