using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JB_ToggleButton : MonoBehaviour
{
    public delegate void OnClickAction();
    public static event OnClickAction OnClicked;

    private bool toggle = false;

   public void ToggleInfo(GameObject infoboard)
    {
        toggle = !toggle;

        infoboard.SetActive(toggle);

        OnClicked();
    }
}
