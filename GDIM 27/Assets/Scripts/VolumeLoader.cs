using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeLoader : MonoBehaviour
{
    private FMOD.Studio.Bus bus;

    // Start is called before the first frame update
    void Start()
    {
        bus = FMODUnity.RuntimeManager.GetBus("bus:/");

        if(!PlayerPrefs.HasKey("volume")) // if there is no setting
        {
            //PlayerPrefs.SetFloat("volume", 1);
            //Load();
            Debug.Log("No Sound Setting!");
        }
        else
        {
            Load();
        }
    }

    private void Load()
    {
        bus.setVolume(PlayerPrefs.GetFloat("volume"));
    }
}
