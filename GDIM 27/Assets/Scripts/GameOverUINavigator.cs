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

    private SaveBetweenScenes _saveBetweenScenes;

    void Start()
    {
        _saveBetweenScenes = GameObject.Find("SaveBetweenScenes").GetComponent<SaveBetweenScenes>();
        GameObject screen = _saveBetweenScenes.PlayerWon ? _winScreenObject : _loseScreenObject;
        screen.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        _saveBetweenScenes.Replay = true;
        _saveBetweenScenes.FirstTime = false;
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
