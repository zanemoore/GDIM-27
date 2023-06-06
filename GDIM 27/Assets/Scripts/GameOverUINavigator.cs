using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUINavigator : MonoBehaviour
{
    [SerializeField] GameObject _winScreenObject;
    [SerializeField] GameObject _loseScreenObject;
    [SerializeField] List<string> sceneMgmtList;

    public FMODUnity.StudioEventEmitter hover;
    public FMODUnity.StudioEventEmitter click;

    void Start()
    {
        SaveBetweenScenes saveBetweenScenes = GameObject.Find("SaveBetweenScenes").GetComponent<SaveBetweenScenes>();
        GameObject screen = saveBetweenScenes.PlayerWon ? _winScreenObject : _loseScreenObject;
        screen.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }


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
