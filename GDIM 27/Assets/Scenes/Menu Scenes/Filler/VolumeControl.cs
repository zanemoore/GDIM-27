using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    FMOD.Studio.Bus AllSFX;

    float allVolume = 0.5f;

    void Awake()
    {
        AllSFX = FMODUnity.RuntimeManager.GetBus("bus:/All SFX");

        SaveBetweenScenes saveBtwnScenes = GameObject.Find("SaveBetweenScenes")
            .GetComponent<SaveBetweenScenes>();
    }

    public void MasterVolumeLevel(float newMasVol)
    {
        allVolume = newMasVol;
        AllSFX.setVolume(allVolume);
    }
}
