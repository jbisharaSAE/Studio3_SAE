using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RZ_CameraResize : MonoBehaviour
{
    //Assign this Camera in the Inspector
    public Camera m_OrthographicCamera;

    // Start is called before the first frame update
    void Start()
    {
        //Set the size of the viewing volume you'd like the orthographic Camera to pick up (5)
        m_OrthographicCamera.orthographicSize = Screen.width/19;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
