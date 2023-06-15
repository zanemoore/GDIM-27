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
    [SerializeField] private GameObject _playerCapsule;
    [SerializeField] private VideoPlayer _openingConversationVideo;
    [SerializeField] private float _timeInOpeningConvoToStartWhiteNoise;
    [SerializeField] private Phone phone; 


    void Start()
    {
        _openingConversationVideo.loopPointReached += DeleteLevelChanger;

        skipInstructions.text = string.Format("Press {0} to Skip.", _skipButton.ToString());

        LockControls();
    }


    void Update()
    {
        if (_openingConversationVideo.time >= _timeInOpeningConvoToStartWhiteNoise && !whiteNoise.IsPlaying())
        {
            whiteNoise.startNoise();
            skipInstructions.text = "";
        }

        if (Input.GetKeyDown(_skipButton))
        {
            DeleteLevelChanger(_openingConversationVideo);
        }
    }


    private void DeleteLevelChanger(VideoPlayer vp)
    {
        if (!whiteNoise.IsPlaying())
        {
            whiteNoise.startNoise();
        }

        phone.startTimer = true;

        skipInstructions.text = "";

        UnlockControls();

        Destroy(gameObject);
    }


    private void LockControls()
    {
        _playerCapsule.GetComponent<FirstPersonController>().enabled = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void UnlockControls()
    {
        _playerCapsule.GetComponent<FirstPersonController>().enabled = true;
        Cursor.visible = false;
    }
}
