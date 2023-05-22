using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconController : MonoBehaviour
{
    // Access awareness value text
    [SerializeField] Slider dangerSlider;
    float dangerNum;

    // Animator for the danger icon
    [SerializeField] Animator dangerIconAnim;

    // Slider Fill
    [SerializeField] GameObject sliderFillGO;
    Image sliderFill;

    void Start()
    {
        // Get fill color
        sliderFill = sliderFillGO.GetComponent<Image>();
    }

    void Update()
    {
        // Setting awareness value variables
        dangerNum = dangerSlider.value * 100;

        // Update DangerIndicator
        dangerIconAnim.SetFloat("DangerIndicator", dangerNum);

        // Update Color of Fill
        if (dangerNum > 60)
            // red
            sliderFill.color = new Color32(192, 0, 0, 255);
        else if (dangerNum > 30)
            // yellow
            sliderFill.color = new Color32(255, 208, 59, 255);
        else if (dangerNum < 30)
            // white
            sliderFill.color = new Color32(255, 255, 255, 255);
    }
}
