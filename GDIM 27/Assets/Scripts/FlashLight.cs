
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlashLight : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    private PlayerInput _input;

    [SerializeField]
    private GameObject flashLight;


    void Start()
    {
        _input.actions["Flashlight"].started += ToggleFlashlight;
    }

    public void ToggleFlashlight(InputAction.CallbackContext context)
    { 
        flashLight.SetActive(!flashLight.activeInHierarchy);
    }

    private void OnDisable()
    {
        if (_input != null)
            _input.actions["Flashlight"].started -= ToggleFlashlight;   
    }
}
