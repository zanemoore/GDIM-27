using Cinemachine;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Windows;

public class Hiding : MonoBehaviour
{
    public GameObject cameraFlashLight;
    public GameObject PlayerFlashLight;
    public FlashLight FL; 

    public FirstPersonController FPC;
    public GameObject oldCamera;
    public GameObject oldGameObject;
    public float seconds;

    private GameObject viewObject;
    private CinemachineVirtualCamera CM;

    private bool switched = false;
    public bool allowed = false;
    private GameObject hideableObject;
    public GameObject body;

    public bool isHidden = false; 
    public FMODUnity.StudioEventEmitter breathingEmitter;

    [SerializeField]
    private PlayerInput _input;

    private void Start()
    {
        _input.actions["Hide"].started += ToggleCamera;
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Hideable")
        {
            hideableObject = other.gameObject;
            allowed = true;
            Debug.Log("ACTUALLY ENTERED");
        }

        Debug.Log("OTHER ENTER");
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Hideable")
        {
            hideableObject = null;
            allowed = false;
        }
    }

    public void ToggleCamera(InputAction.CallbackContext context)
    {
        Debug.Log("Calling");
        if (!allowed)
            return;

        Debug.Log("Passed the function");
        if (!switched)
        {
            CM = hideableObject.gameObject.transform.GetChild(0).GetComponent<CinemachineVirtualCamera>();
            viewObject = hideableObject.gameObject.transform.GetChild(1).gameObject;
            CM.Priority = 11;
            FPC.CinemachineCameraTarget = CM.gameObject;
            cameraFlashLight.SetActive(FL.flashLight.activeInHierarchy);
            FL.flashLight = cameraFlashLight;
            FPC.currentCamera = viewObject;
            PlayerFlashLight.SetActive(false);
            body.SetActive(false);
            isHidden = true;
            _input.actions["Move"].Disable();
            breathingEmitter.Play();
        }
        else
        {
            CM.Priority = 9;
            FPC.CinemachineCameraTarget = oldGameObject;
            FPC.currentCamera = oldCamera;
            StartCoroutine(WaitForAnimation());
            isHidden = false;
            PlayerFlashLight.SetActive(FL.flashLight.activeInHierarchy);
            FL.flashLight = PlayerFlashLight;
            cameraFlashLight.SetActive(false);
            _input.actions["Move"].Enable();

            stopBreathingSound();
        }

        switched = !switched;
    
    }

    public void stopBreathingSound()
    {
        breathingEmitter.Stop();
    }

    private void OnDisable()
    {
        if (_input != null)
            _input.actions["Hide"].started -= ToggleCamera;
    }


    IEnumerator WaitForAnimation()
    { 
        yield return new WaitForSeconds(seconds);
        body.SetActive(true);

    }

    void OnDestroy()
    {
        stopBreathingSound();
    }

}
