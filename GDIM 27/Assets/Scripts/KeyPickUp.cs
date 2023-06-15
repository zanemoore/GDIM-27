using Sounds;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KeyPickUp : MonoBehaviour
{
    [SerializeField] private FMODUnity.StudioEventEmitter keyEmitter;
    [SerializeField] private FMODUnity.StudioEventEmitter lockedEmitter;
    [SerializeField] private FMODUnity.StudioEventEmitter unlockedEmitter;
    [SerializeField] private float soundRange;

    [SerializeField] private float maxInteractRange;
    [SerializeField] private GameObject[] keys;

    [SerializeField] private GameObject mascot;
    [SerializeField] private FMODUnity.StudioEventEmitter zotEmitter; // remember to add directional zots

    [SerializeField] private TextMeshProUGUI uiInstructions;
    [SerializeField] private float timeToAppear = 2f;
    [SerializeField] private List<string> openingNonExitDoorTexts;

    [SerializeField] private GameObject doorReticle;
    [SerializeField] private GameObject exitReticle;
    [SerializeField] private GameObject normalReticle;
    [SerializeField] private Hiding hide;
    [SerializeField] private PauseMenu pause;

    private float timeWhenDisappear;
    private bool hasKey;
    private int numKeysTried;


    void Start()
    {
        UnityEngine.Random.InitState(27);

        hasKey = false;
        numKeysTried = 0;

        timeToAppear = 56f;  // This is 56f to make sure the text stays up past the entirety of the cutscene (good idea to drag in video and do timeToAppear += video.lenght?) - Diego
        SetText("I gotta find an exit.\n[Find an Exit Door]");
        timeToAppear = 2f;
    }


    void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, maxInteractRange))
        {
            GameObject obj = hit.collider.gameObject;

            if (obj.tag == "Key" && Input.GetMouseButtonDown(0))
            {
                PickUpKey(obj);
            }
            else if (obj.layer == LayerMask.NameToLayer("Door") && !hide.isHidden)
            {
                if (obj.tag == "Exit" && !hide.allowed)
                {
                    exitReticle.SetActive(true);
                    normalReticle.SetActive(false);
                }
                else if (obj.tag == "Untagged" && !hide.allowed)
                {
                    doorReticle.SetActive(true);
                    normalReticle.SetActive(false);
                }
                if (Input.GetMouseButtonDown(0) && !pause.isPaused)
                {
                    TryOpenDoor(obj);
                }
            }
            else
            {
                if (obj.tag == "Untagged" && !hide.allowed)
                {
                    normalReticle.SetActive(true);
                }
                exitReticle.SetActive(false);
                doorReticle.SetActive(false);

            }


        }
        else if (Physics.Raycast(ray, out hit, maxInteractRange + 10f))
        {
            GameObject obj = hit.collider.gameObject;
            if (obj.tag != "Throwable" && !hide.allowed)
            {
                normalReticle.SetActive(true);
            }
            exitReticle.SetActive(false);
            doorReticle.SetActive(false);

        }
    

        if (Time.time >= timeWhenDisappear)
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


    private void TryOpenDoor(GameObject door)
    {
        if (door.tag == "Exit")
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
                    GameObject.Find("SaveBetweenScenes").GetComponent<SaveBetweenScenes>().PlayerWon = true;
                    SceneManager.LoadScene("Game Over");
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
                    // zotEmitter.Play(); commented out; old version of beginning zot - dare
                }
                else
                {
                    SetText("Wrong key...\nWho locks emergency doors anyways?");
                }
            }
        }
        else
        {
            if (!lockedEmitter.IsPlaying())
            {
                lockedEmitter.Play();
            }

            int rndInt = UnityEngine.Random.Range(0, openingNonExitDoorTexts.Count);
            SetText(openingNonExitDoorTexts[rndInt]);
        }
    }


    private void SpawnKey(int keyNum)
    {
        keys[keyNum].SetActive(true);
    }


    private void SetText(string text)
    {
        uiInstructions.text = text;
        uiInstructions.enabled = true;
        timeWhenDisappear = Time.time + timeToAppear;
    }
}
