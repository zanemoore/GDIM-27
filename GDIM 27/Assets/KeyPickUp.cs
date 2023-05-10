using Sounds;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private bool hasKey;
    private int numKeysTried;


    void Start()
    {
        hasKey = false;
        numKeysTried = 0;
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
            else if (obj.tag == "Exit" && Input.GetMouseButtonDown(0))
            {
                TryOpenDoor();
            }
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
                // Win Condition
                print("Freedom!");
            }
            else
            {
                // UI saying wrong key
                SpawnKey(numKeysTried);
            }

            hasKey = false;
        }
        else
        {
            if (numKeysTried == 0)
            {
                // UI tells you what to do
                SpawnKey(0);
                print("key should have spawned, weird champ");
                mascot.SetActive(true);
            }
            else
            {
                // UI says rip bozo
            }
        }
    }


    private void SpawnKey(int keyNum)
    {
        keys[keyNum].SetActive(true);
    }
}
