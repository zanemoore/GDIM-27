using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionEye : MonoBehaviour
{
    // Get sprites for eyes
    [SerializeField] Sprite eyeClosed;
    [SerializeField] Sprite eyeOpen;

    // Get Arrow's Image component
    [SerializeField] Image image;

    void Start()
    {
        // Start off with closed eye
        image.sprite = eyeClosed;
    }

    void Update()
    {
        // If dropdown is open, open eye
        if (GameObject.Find("Dropdown List"))
        {
            image.sprite = eyeOpen;
        }
        // else, close eye
        else
        {
            image.sprite = eyeClosed;
        }
    }
}
