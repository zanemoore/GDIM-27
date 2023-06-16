
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlashLight : MonoBehaviour
{
    [SerializeField]
    private PlayerInput _input;

    public GameObject flashLight;
    public FMODUnity.StudioEventEmitter flashlightEmitter;

    private bool _isOpeningConvoPlaying;

    [SerializeField] private PauseMenu pauseMenu;
    void Start()
    {
        _input.actions["Flashlight"].started += ToggleFlashlight;

        if (!_isOpeningConvoPlaying)  // This IF ensures bool isn't overwritten if SetIsOpeningConvoPlaying was called first
        {
            _isOpeningConvoPlaying = false;  // I'm defaulting it to false, but value should only be updated in LevelChanger - Diego
        }
    }

    public void ToggleFlashlight(InputAction.CallbackContext context)
    {
        if (pauseMenu.isPaused == false && _isOpeningConvoPlaying == false)
        {
            flashLight.SetActive(!flashLight.activeInHierarchy);
            flashlightEmitter.Play();
        }
    }

    private void OnDisable()
    {
        if (_input != null)
            _input.actions["Flashlight"].started -= ToggleFlashlight;
    }


    public void SetIsOpeningConvoPlaying(bool isPlaying)  // Adding this to ensure Flashlight doesn't turn on during opening convo. Called in LevelChanger - Diego
    {
        _isOpeningConvoPlaying = isPlaying;
    }
}