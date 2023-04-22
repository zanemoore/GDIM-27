using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Phone : MonoBehaviour
{
    [SerializeField]
    private PlayerInput _input;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private bool phoneActive;
    // Start is called before the first frame update
    void Start()
    {
        phoneActive = false;
        _input.actions["Phone"].started += TogglePhone;
    }

    public void TogglePhone(InputAction.CallbackContext context)
    {
        phoneActive = !phoneActive;
        animator.SetBool("On", phoneActive);
    }

    private void OnDisable()
    {
        _input.actions["Phone"].started -= TogglePhone;
    }

}
