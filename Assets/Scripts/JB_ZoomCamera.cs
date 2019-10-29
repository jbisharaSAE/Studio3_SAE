using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class JB_ZoomCamera : NetworkBehaviour
{
    public float zoomOutMin = 1f;
    public float zoomOutMax = 8f;

    Vector3 touchStart;

    public bool shipsLocked = false;

   
    // Update is called once per frame
    void Update()
    {
        if (!hasAuthority)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if (shipsLocked)
        {
            if (Input.GetMouseButton(0))
            {
                Vector3 direction = touchStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);

                //direction = new Vector3(Mathf.Clamp(direction.x, -143, 143), Mathf.Clamp(direction.y, -86, 86), -10f);

                Camera.main.transform.position += direction;

                Camera.main.transform.position = new Vector3(Mathf.Clamp(Camera.main.transform.position.x, -100f, 100f),
                                                             Mathf.Clamp(Camera.main.transform.position.y, -50f, 50f), -10f);
            }
        }
        

        if(Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;

            Zoom(difference * 0.07f);
        }
        
    }

    private void Zoom(float increment)
    {
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - increment, zoomOutMin, zoomOutMax);
        
    }
}
