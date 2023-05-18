using Sounds;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KeyPickUp : MonoBehaviour
{
    [SerializeField]
    private FMODUnity.StudioEventEmitter dropEmitter;
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
        timeToAppear = 50f;
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
        Destroy(key);
        hasKey = true;

        // doesn't allow for sounds to overlap
        if (dropEmitter == null || dropEmitter.IsPlaying())
        {
            return;
        }

        dropEmitter.Play();

        var sound = new ObjectSound(transform.position, soundRange);

        ObjectSoundManager.MakeSound(sound);
    }


    private void TryOpenDoor()
    {
        if (hasKey)
        {
            numKeysTried++;

            if (numKeysTried == keys.Length)
            {
                SetText("MADE IT OUT");
                SceneManager.LoadScene("MainMenu");  // Temp for when you win - Diego
            }
            else
            {
                SetText("Dammit, wrong key...\nWhere's the actual key?!");
                SpawnKey(numKeysTried);
            }

            hasKey = false;
        }
        else
        {
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
