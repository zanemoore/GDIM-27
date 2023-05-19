using Sounds;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KeyPickUp : MonoBehaviour
{
    [SerializeField]
    private FMODUnity.StudioEventEmitter keyEmitter;
    [SerializeField]
    private FMODUnity.StudioEventEmitter lockedEmitter;
    [SerializeField]
    private FMODUnity.StudioEventEmitter unlockedEmitter;
    [SerializeField]
    private float soundRange;
    [SerializeField]
    private GameObject[] keys;
    [SerializeField]
    private GameObject mascot;
    [SerializeField]
    private TextMeshProUGUI uiInstructions;
    [SerializeField]
    private float timeToAppear = 2f;
    private float timeWhenDisappear;

    private bool hasKey;
    private int numKeysTried;


    void Start()
    {
        hasKey = false;
        numKeysTried = 0;
        timeToAppear = 50f;  // This is 50f to make sure the text stays up past the entirety of the cutscene
        SetText("I gotta find an exit.\n[Find an Exit Door]");
        timeToAppear = 2f;
    }


    void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            GameObject obj = hit.collider.gameObject;

            if (obj.tag == "Key" && Input.GetMouseButtonDown(0))
            {
                PickUpKey(obj);
               
            }
            else if (obj.tag == "Exit")
            {
                // SHOW DOOR RETICLE
                if (Input.GetMouseButtonDown(0))
                {
                    TryOpenDoor();
                }
            }
        }

        if (uiInstructions.enabled && (Time.time >= timeWhenDisappear))
        {
            uiInstructions.enabled = false;
        }
    }


    private void PickUpKey(GameObject key)
    {
        hasKey = true;

        // doesn't allow for sounds to overlap
        if (keyEmitter == null || keyEmitter.IsPlaying())
        {
            return;
        }

        keyEmitter.Play();

        var sound = new ObjectSound(transform.position, soundRange);

        ObjectSoundManager.MakeSound(sound);

        Destroy(key);
    }


    private void TryOpenDoor()
    {
        if (hasKey)
        {
            numKeysTried++;

            if (numKeysTried == keys.Length)
            {
                if (!unlockedEmitter.IsPlaying())
                {
                    unlockedEmitter.Play();
                }

                Cursor.lockState = CursorLockMode.None;
                SceneManager.LoadScene("MainMenu");  // Temp for when you win - Diego
            }
            else
            {
                if (!lockedEmitter.IsPlaying())
                {
                    lockedEmitter.Play();
                }
                SetText("Dammit, wrong key...\nWhere's the actual key?!");
                SpawnKey(numKeysTried);
            }

            hasKey = false;
        }
        else
        {
            if (!lockedEmitter.IsPlaying())
            {
                lockedEmitter.Play();
            }

            if (numKeysTried == 0)
            {
                SetText("Emergency door's locked?\nMaybe there's a key...");
                SpawnKey(0);
                mascot.SetActive(true);
            }
            else
            {
                SetText("Wrong key...\nWho locks emergency doors anyways?");
            }
        }
    }


    private void SpawnKey(int keyNum)
    {
        keys[keyNum].SetActive(true);
    }


    private void EnableText()
    {
        uiInstructions.enabled = true;
        timeWhenDisappear = Time.time + timeToAppear;
    }


    private void SetText(string text)
    {
        uiInstructions.text = text;
        EnableText();
    }
}
