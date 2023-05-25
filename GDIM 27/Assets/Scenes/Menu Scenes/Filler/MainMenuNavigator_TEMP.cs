using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//ALL THIS IS TEMP CODE MEANT TO TEST MENU SWITCHING
public class MainMenuNavigator_TEMP : MonoBehaviour
{
    [SerializeField] GameObject mainMenu;//A game object representing base menu
    [SerializeField] GameObject settingsMenu;//A game object representing settings menu
    [SerializeField] GameObject levelSelectMenu; //A game object representing level select menu
    [SerializeField] string mainScene; //Main scene
    [SerializeField] string loadScene; //Scene loaded next

    [SerializeField]
    List<string> sceneMgmtList;

    [SerializeField]
    List<int> resWidths;//A list of screen size widths
    [SerializeField]
    List<int> resHeights;//A list of screen size heights

    public FMODUnity.StudioEventEmitter hover;
    public FMODUnity.StudioEventEmitter click;

    // plays button sounds - dare
    public void hoverSound()
    {
        hover.Play();
    }

    public void clickSound()
    {
        click.Play();
    }

    //Sets Base Menu to Active
    public void GoToBase()
    {
        mainMenu.SetActive(true);
        settingsMenu.SetActive(false);
        levelSelectMenu.SetActive(false);
    }

    //Opens Level Select Menu
    public void GoToLevelSelect()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(false);
        levelSelectMenu.SetActive(true);
    }

    //Opens setting menu
    public void GoToSettings()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
        levelSelectMenu.SetActive(false);
    }

    //TEMP CODE BELOW, NEEDS REFACTORING
    public void ToUCI()
    {
        SceneManager.LoadScene(sceneMgmtList[0]);
    }

    public void ToUCR()
    {
        SceneManager.LoadScene(sceneMgmtList[1]);
    }

    public void ToUCSD()
    {
        SceneManager.LoadScene(sceneMgmtList[2]);
    }


    //Quits game
    public void OnQuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    //Sets the screensize based on a selected option
    public void SetScreenRes(int index)
    {
        bool isFullscreen = Screen.fullScreen;
        int width = resWidths[index];
        int height = resHeights[index];

        Screen.SetResolution(width, height, isFullscreen);
    }

    //Determines fullscreen 
    public void SetFullscreen(bool _fullscreen)
    {
        Screen.fullScreen = _fullscreen;
    }

}
