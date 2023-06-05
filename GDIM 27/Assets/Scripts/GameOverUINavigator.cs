using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUINavigator : MonoBehaviour
{
    [SerializeField] GameObject _jumpScareObject;
    [SerializeField] GameObject _winScreenObject;
    [SerializeField] GameObject _loseScreenObject;

    [SerializeField]
    List<string> sceneMgmtList;

    public FMODUnity.StudioEventEmitter hover;
    public FMODUnity.StudioEventEmitter click;


    // Adding this to Start to ensure cursor is visible at beginning
    void Start()
    {
        Cursor.visible = true;
    }


    private void Update()
    {

    }


    // plays button sounds - dare
    public void hoverSound()
    {
        hover.Play();
    }


    public void clickSound()
    {
        click.Play();
    }


    public void loadFinalScene()
    {
        SceneManager.LoadScene(sceneMgmtList[0]);
    }


    public void loadMainMenu()
    {
        SceneManager.LoadScene(sceneMgmtList[1]);
    }
}
