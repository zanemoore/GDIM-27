using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelChanger : MonoBehaviour
{
    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        //Start the coroutine we define below named ExampleCoroutine.
        StartCoroutine(ExampleCoroutine());
        FadeToLevel(1);
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
    }
}
