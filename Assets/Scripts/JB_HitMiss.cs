using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JB_HitMiss : MonoBehaviour
{
    private SpriteRenderer spriteRend;
    private Color color;

    // Start is called before the first frame update
    void Start()
    {
        spriteRend = GetComponent<SpriteRenderer>();
        color = GetComponent<SpriteRenderer>().color;
        StartCoroutine(RaiseAlpha());
    }


    IEnumerator RaiseAlpha()
    {
        float alpha = 0f;

        while (alpha<1)
        {

            alpha += 0.05f;
            
            color.a = alpha;

            spriteRend.color = color;
            yield return null;
        }
        
    }
}
