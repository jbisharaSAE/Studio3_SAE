using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class CS_SceneChange : MonoBehaviour 
{

    public GameObject InfoGraphic;
    public GameObject MainMenuHolder;

    private void Start()
    {
        MainMenuHolder.gameObject.SetActive(true);
        InfoGraphic.gameObject.SetActive(false);
    }

    public void LoadAnyScene(int index)
    {
        SceneManager.LoadScene(index);

        
    }

    public void ActiavateInfo()
    {
        MainMenuHolder.gameObject.SetActive(false);
        InfoGraphic.gameObject.SetActive(true);
    }

    public void DeactiavateInfo()
    {
        MainMenuHolder.gameObject.SetActive(true);
        InfoGraphic.gameObject.SetActive(false);
    }

    public void QuitST()
    {
        Application.Quit();
    }

}
