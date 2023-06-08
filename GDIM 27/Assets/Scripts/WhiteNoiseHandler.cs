using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteNoiseHandler : MonoBehaviour
{
    public FMODUnity.StudioEventEmitter wn;

    public void startNoise()
    {
        wn.Play();
    }


    public void stopNoise()
    {
        wn.Stop();
    }


    public bool IsPlaying()
    {
        return wn.IsPlaying();
    }


    void OnDestroy()
    {
        stopNoise();
    }
}
