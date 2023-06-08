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
    [SerializeField] GameObject loadingScreen; //A game object representing level select menu

    [SerializeField]
    List<int> resWidths;//A list of screen size widths
    [SerializeField]
    List<int> resHeights;//A list of screen size heights

    public FMODUnity.StudioEventEmitter hover;
    public FMODUnity.StudioEventEmitter click;

    // Adding this to Start to ensure cursor is visible at beginning
    void Start()
    {
        Cursor.visible = true;
    }

    private void Update()
    {
        if (settingsMenu.activeSelf)
        {
            // Debug.Log("Settings is active.");
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GoToBase();
            }
        }
    }

    // plays btn sounds when hovering - dare
    public void hoverSound()
    {
        hover.Play();
    }


    // plays btn sounds when clicking - dare
    public void clickSound()
    {
        click.Play();
    }

    //Sets Base Menu to Active
    public void GoToBase()
    {
        mainMenu.SetActive(true);
        settingsMenu.SetActive(false);
    }

    //Opens setting menu
    public void GoToSettings()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    //Loads the next scene in the build settings asynchronously
    public void LoadScreen(int sceneId)
    {
        StartCoroutine(LoadSceneAsync(sceneId));
    }

    //Enumerator for showing loading screen panel
    IEnumerator LoadSceneAsync(int sceneId)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneId);

        loadingScreen.SetActive(true);
        mainMenu.SetActive(false);
        settingsMenu.SetActive(false);

        while (!operation.isDone)
        {
            yield return null;
        }
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
