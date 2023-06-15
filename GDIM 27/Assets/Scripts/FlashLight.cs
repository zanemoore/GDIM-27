
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

    [SerializeField] private PauseMenu pauseMenu;
    void Start()
    {
        _input.actions["Flashlight"].started += ToggleFlashlight;
    }

    public void ToggleFlashlight(InputAction.CallbackContext context)
    {
        if (pauseMenu.isPaused == false)
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
}