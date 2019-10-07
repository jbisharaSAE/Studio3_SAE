using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JB_Button : MonoBehaviour, IPointerDownHandler
{
    private void OnMouseDown()
    {
        Debug.Log("button detected");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        
        
    }

    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            
        }
    }
}
