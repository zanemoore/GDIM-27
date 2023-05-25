using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelChanger : MonoBehaviour
{
    [SerializeField]
    private GameObject openingConversation;
    [SerializeField]
    private WhiteNoiseHandler whiteNoise;
    [SerializeField]
    private TextMeshProUGUI skipInstructions;
    [SerializeField]
    private GameObject fpsController;

    public Animator animator;
    private KeyCode skipButton;

    void Start()
    {
        skipButton = KeyCode.Space; // If you want to change key to skip, just change it to KeyCode.WHATEVER
        LockControls();
        skipInstructions.text = string.Format("Press {0} to Skip.", skipButton.ToString());
        //Start the coroutine we define below named ExampleCoroutine.
        StartCoroutine(ExampleCoroutine());
        FadeToLevel(1);
    }


    void Update()
    {
        if (Input.GetKeyDown(skipButton))
        {
            DeleteLevelChanger();
        }
    }


    IEnumerator ExampleCoroutine()
    {
        //Print the time of when the function is first called.
        Debug.Log("Started Coroutine at timestamp : " + Time.time);

        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(45);
        DeleteLevelChanger();

        //After we have waited 5 seconds print the time again.
        Debug.Log("Finished Coroutine at timestamp : " + Time.time);
    }

    public void FadeToLevel(int levelIndex)
    {
        animator.SetTrigger("FadeIn");
        whiteNoise.startNoise();
    }


    private void DeleteLevelChanger()
    {
        UnlockControls();
        Destroy(openingConversation);
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
        Cursor.lockState = CursorLockMode.None;
    }
}
