using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnMouseOverHide : MonoBehaviour
{
    [SerializeField] private GameObject hideReticle;
    [SerializeField] private GameObject normalReticle;
    [SerializeField] private GameObject leaveHideReticle;
    [SerializeField] private GameObject grabReticle;
    [SerializeField] private GameObject doorReticle;
    [SerializeField] private GameObject exitReticle;
    [SerializeField] private Hiding hide;
    public bool mouseOver = false;

    private void OnMouseOver()
    {
        if (hide.allowed == true)
        {
            mouseOver = true;
        }
    }

    private void OnMouseExit()
    {
        mouseOver = false;
    }
}
