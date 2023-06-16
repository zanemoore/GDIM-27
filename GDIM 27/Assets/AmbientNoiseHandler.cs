using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientNoiseHandler : MonoBehaviour
{
    [SerializeField] private FMODUnity.StudioEventEmitter _ambientNoise;
    [SerializeField] private float minTime;
    [SerializeField] private float maxTime;

    private bool _playNoise;
    private float _nextNoiseTime;
    private float _lastTime;


    void Start()
    {
        _playNoise = false;
        _nextNoiseTime = float.PositiveInfinity;
        _lastTime = 0f;
    }


    void Update()
    {
        if (_playNoise && (_nextNoiseTime < Time.time - _lastTime))
        {
            _lastTime = Time.time;
            _nextNoiseTime = UnityEngine.Random.Range(minTime, maxTime);

            _ambientNoise.Play();
        }
    }

    public void StartNoise()
    {
        _playNoise = true;

        _lastTime = Time.time;
        _nextNoiseTime = UnityEngine.Random.Range(minTime, maxTime);
    }


    public void StopNoise()
    {
        _playNoise = false;
        _ambientNoise.Stop();
    }


    void OnDestroy()
    {
        StopNoise();
    }
}
