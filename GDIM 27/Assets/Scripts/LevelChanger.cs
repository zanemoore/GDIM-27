using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class LevelChanger : MonoBehaviour
{
    [SerializeField] private WhiteNoiseHandler whiteNoise;
    [SerializeField] private AmbientNoiseHandler _ambientNoise;
    [SerializeField] private TextMeshProUGUI skipInstructions;
    [SerializeField] private KeyCode _skipButton;
    [SerializeField] private GameObject _playerCapsule;
    [SerializeField] private VideoPlayer _openingConversationVideo;
    [SerializeField] private float _timeInOpeningConvoToStartWhiteNoise;
    [SerializeField] private Phone phone;
    [SerializeField] private FlashLight _flashLight;


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


    private void DeleteLevelChanger(VideoPlayer vp)  // VideoPlayer is a needed argument because this function is subscribed to an Action that requires it - Diego
    {
        if (!whiteNoise.IsPlaying())
        {
            whiteNoise.startNoise();
        }

        _ambientNoise.StartNoise();

        skipInstructions.text = "";

        UnlockControls();

        Destroy(this.gameObject);
    }


    private void LockControls()
    {
        _flashLight.SetIsOpeningConvoPlaying(true);  // Prevents flashlight from being used during opening convo - Diego

        phone.SetIsOpeningConvoPlaying(true);  // Prevents phone from being used during opening convo - Diego

        _playerCapsule.GetComponent<FirstPersonController>().enabled = false;  // Prevents player from moving camera around during opening convo - Diego

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void UnlockControls()
    {
        _flashLight.SetIsOpeningConvoPlaying(false);

        phone.SetIsOpeningConvoPlaying(false);
        phone.startTimer = true;  // Phone time begins once DeleteLevelChanger is called (which is where this should be called as well)

        _playerCapsule.GetComponent<FirstPersonController>().enabled = true;

        Cursor.visible = false;
    }
}
