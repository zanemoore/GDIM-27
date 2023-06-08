using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoVolumeLeader : MonoBehaviour //this is a typo lol; it's supposed to say loader
{
    public UnityEngine.Video.VideoPlayer vp;

    // Start is called before the first frame update
    void Start()
    {
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
        vp.SetDirectAudioVolume(0, PlayerPrefs.GetFloat("volume"));
    }
}
