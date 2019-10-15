using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JB_MovingBackground : MonoBehaviour
{
    public float scrollSpeed = 0.5f;

    private Renderer myRenderer;

    // Start is called before the first frame update
    void Start()
    {
        myRenderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 offset = new Vector2(Time.time * scrollSpeed, 0);

        myRenderer.material.mainTextureOffset = offset;
    }
}
