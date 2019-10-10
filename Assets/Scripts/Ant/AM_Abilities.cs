using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AM_Abilities : MonoBehaviour
{
    public Vector2 targetPos;
    public float blastDeployDelay;
    public GameObject blastProjectile;
    public GameObject selectionTile;
    public List<GameObject> selectionTileGroup = new List<GameObject>();
    public List<GameObject> blastProjectileGroup = new List<GameObject>();

    public bool blastButtonReady;
    public bool deployButtonReady;
    public bool deployHasHappened;

    private void Update()
    {
        Aiming();
        //Deploy();
    }

    public void EnableAiming()
    {
        blastButtonReady = true;
    }

    public void EnableDeploy()
    {
        deployButtonReady = true;
    }

    //player aiming
    public void Aiming()
    {
        if (blastButtonReady && !deployHasHappened)
        {
            //Set the button interactiable to false once clicked
            GameObject blastButton = GameObject.Find("Blast Button");
            Button blastButtonComponent = blastButton.GetComponent<Button>();
            blastButtonComponent.interactable = false;

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit) && Input.GetMouseButtonDown(0))
            {
                //Get access to object the player clicked
                GameObject hitObject = hit.collider.gameObject;
                //Get access to the tile position variable inside of the script in the object
                JB_Tile tileInfoScript = hitObject.GetComponent<JB_Tile>();

                //Set that position in a variable
                targetPos = tileInfoScript.tilePosition;

                //Spawn the projectile
                GameObject mySelectionTile = Instantiate(selectionTile, hitObject.transform.position, Quaternion.identity);

                selectionTileGroup.Add(mySelectionTile);
            }
        }
    }

    //Deploying attack
    public void Deploy()
    {
        deployHasHappened = true;

        //Set the button interactiable to false once clicked
        GameObject deployButton = GameObject.Find("Deploy Button");
        Button deployButtonComponent = deployButton.GetComponent<Button>();
        deployButtonComponent.interactable = false;

        foreach (GameObject mySelectionTile in selectionTileGroup)
        {
            Debug.Log("spawning proj");
            GameObject myBlastProjectile = Instantiate(blastProjectile, transform.position, Quaternion.identity);

            //Add the projectile to a list
            blastProjectileGroup.Add(myBlastProjectile);
        }
    }
}
