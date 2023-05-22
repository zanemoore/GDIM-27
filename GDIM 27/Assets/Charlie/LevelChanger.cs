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

    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        skipInstructions.text = "Press Any Key to Skip.";
        //Start the coroutine we define below named ExampleCoroutine.
        StartCoroutine(ExampleCoroutine());
        FadeToLevel(1);
    }


    void Update()
    {
        if (Input.anyKey)
        {
            Cursor.lockState = CursorLockMode.None;
            Destroy(openingConversation);
            Destroy(this.gameObject);
        }
    }


    IEnumerator ExampleCoroutine()
    {
        //Print the time of when the function is first called.
        Debug.Log("Started Coroutine at timestamp : " + Time.time);

        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(45);

        //After we have waited 5 seconds print the time again.
        Debug.Log("Finished Coroutine at timestamp : " + Time.time);
    }

    public void FadeToLevel(int levelIndex)
    {
        animator.SetTrigger("FadeIn");
        whiteNoise.startNoise();
        Cursor.lockState = CursorLockMode.None;
    }
}
