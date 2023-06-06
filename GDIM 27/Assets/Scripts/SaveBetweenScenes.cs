using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveBetweenScenes : MonoBehaviour
{
    public static SaveBetweenScenes instance;

    [SerializeField] private bool _playerWon;
    [SerializeField] private bool _replay;

    public bool PlayerWon { get { return _playerWon; } set { _playerWon = value; } }
    public bool Replay { get { return _replay; } set { _replay = value; } }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
}
