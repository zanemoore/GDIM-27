using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class LevelChanger : MonoBehaviour
{
    [SerializeField] private WhiteNoiseHandler whiteNoise;
    [SerializeField] private TextMeshProUGUI skipInstructions;
    [SerializeField] private KeyCode _skipButton;
    [SerializeField] private GameObject fpsController;
    [SerializeField] private GameObject _openingConversationObject;
    [SerializeField] private VideoPlayer _openingConversationVideo;

    public Animator animator;
    private KeyCode skipButton;

    void Start()
    {
        _openingConversationVideo.loopPointReached += DeleteLevelChanger;

        // skipButton = KeyCode.Space; // If you want to change key to skip, just change it to KeyCode.WHATEVER
        skipInstructions.text = string.Format("Press {0} to Skip.", _skipButton.ToString());

        LockControls();
    }


    void Update()
    {
        if (Input.GetKeyDown(_skipButton))
        {
            DeleteLevelChanger(_openingConversationVideo);
        }
    }


    private void DeleteLevelChanger(VideoPlayer vp)
    {
        whiteNoise.startNoise();
        UnlockControls();
        _openingConversationObject.SetActive(false);
        Destroy(this.gameObject);
    }


    private void LockControls()
    {
        fpsController.GetComponent<FirstPersonController>().enabled = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void UnlockControls()
    {
        fpsController.GetComponent<FirstPersonController>().enabled = true;
        Cursor.visible = false;
    }
}
