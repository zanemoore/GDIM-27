using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BankHandler : MonoBehaviour // this should be called meter handler lol
{
    public FMODUnity.StudioEventEmitter convoEmitter;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!convoEmitter.IsPlaying())
        {
            convoEmitter.SetParameter("isCutscene", 0);
        }
    }
}
