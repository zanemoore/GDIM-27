using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using Unity.XR.GoogleVr;

public class Phone : MonoBehaviour
{
    [SerializeField] private PlayerInput _input;
    [SerializeField] private Animator animator;
    [SerializeField] private TMP_Text clockText;
    [SerializeField] private float time;
    [SerializeField] private float timeScale; // how fast time should pass in game

    public bool recievedText;
    private bool phoneActive;

    void Start()
    {
        phoneActive = false;
        _input.actions["Phone"].started += TogglePhone;
    }

    private void Update()
    {
        time += Time.deltaTime * timeScale;
        PhoneClock();
    }

    public void TogglePhone(InputAction.CallbackContext context)
    {
        phoneActive = !phoneActive;
        animator.SetBool("On", phoneActive);
    }

    public void PhonePeek()
    {
        if(recievedText == true && phoneActive == false)
        {
            animator.SetBool("Peek", true);
        }

        StartCoroutine(PeekTime());
    }

    IEnumerator PeekTime()
    {
        yield return new WaitForSeconds(5f);
        animator.SetBool("Peek", false);
    }

    private void PhoneClock()
    {
        int hours = Mathf.FloorToInt(time / 3600f);
        int minutes = Mathf.FloorToInt((time - hours * 3600f) / 60f);

        clockText.text = string.Format("{0:00}:{1:00}", hours, minutes);
    }

    private void OnDisable()
    {
        _input.actions["Phone"].started -= TogglePhone;
    }

}
