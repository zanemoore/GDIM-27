using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChairScript : MonoBehaviour
{
    [SerializeField] private FMODUnity.StudioEventEmitter chairEmitter;
    [SerializeField] private bool isStandingOn = false;
    public bool startGameAttenuation = false;
    private bool waited = false;
    private float timer = 0;
    private float waitTime = 1;

    void Update()
    {
        if (!waited) // controls sound at the very beginning
        {
            waitALittle();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!startGameAttenuation)
        {
            return; // ignores the first collision at the start of the game
        }

        if (collision.gameObject.tag == "Player")
        {
            isStandingOn = true; // make no sound
        }
        else if (collision.gameObject.tag == "Mascot")
        {
            // make no sound
        }
        else
        {
            if (startGameAttenuation)
            {
                if (chairEmitter.IsPlaying())
                {
                    return;
                }

                chairEmitter.Play();
            }
        }
    }

    private void waitALittle() // turns on sound after a little
    {
        timer += Time.deltaTime;

        if (timer >= waitTime)
        {
            waited = true;
            startGameAttenuation = true;
        }
    }
}
