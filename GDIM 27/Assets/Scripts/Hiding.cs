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
    public FirstPersonController FPC;
    public GameObject oldCamera;
    public GameObject oldGameObject;
    public float seconds;

    private GameObject viewObject;
    private CinemachineVirtualCamera CM;

    private bool switched = false;
    private bool allowed = false;
    private GameObject hideableObject;
    public GameObject body;

    public bool isHidden = false; 

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
        if (!allowed)
            return; 
        if (!switched)
        {
            CM = hideableObject.gameObject.transform.GetChild(0).GetComponent<CinemachineVirtualCamera>();
            viewObject = hideableObject.gameObject.transform.GetChild(1).gameObject;
            CM.Priority = 11;
            FPC.CinemachineCameraTarget = CM.gameObject;
            FPC.currentCamera = viewObject;
            body.SetActive(false);
            isHidden = true;
        }
        else
        {
            CM.Priority = 9;
            FPC.CinemachineCameraTarget = oldGameObject;
            FPC.currentCamera = oldCamera;
            StartCoroutine(WaitForAnimation());
            isHidden = false;
        }

        switched = !switched;
    
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
}
