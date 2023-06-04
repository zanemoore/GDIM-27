using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeAdjuster : MonoBehaviour
{
    private FMOD.Studio.Bus bus;
    public Slider volumeSlider;


    // Start is called before the first frame update
    void Start()
    {
        bus = FMODUnity.RuntimeManager.GetBus("bus:/");

        if(!PlayerPrefs.HasKey("volume"))
        {
            PlayerPrefs.SetFloat("volume", 1);
            Load();
        }
        else
        {
            Load();
        }
    }

    public void ChangeVolume()
    {
        bus.setVolume(volumeSlider.value);
        Save();
    }

    private void Load()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("volume");
    }

    private void Save()
    {
        PlayerPrefs.SetFloat("volume", volumeSlider.value);
    }
}
