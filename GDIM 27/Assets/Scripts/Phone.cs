using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using Unity.XR.GoogleVr;
using System;

public class Phone : MonoBehaviour
{
    [Serializable]
    public class TextMessage
    {
        public float time;
        [TextArea(5, 10)]
        public string text;
        public string sender;
        public bool sent;
        public bool random;
        public float minTime;
        public float maxTime;
        public bool sound; // does the text play the eerie sfx
    }


    [SerializeField] private PlayerInput _input;
    [SerializeField] private Animator animator;
    [SerializeField] private TMP_Text clockText;
    [SerializeField] private float time;
    [SerializeField] private float timeScale; // how fast time should pass in game
    
    [SerializeField] private Transform textContainer;
    [SerializeField] private GameObject textPrefab;

    public bool recievedText;
    private bool phoneActive;

    [SerializeField] private List<TextMessage> texts = new List<TextMessage>();

    //sound
    public FMODUnity.StudioEventEmitter vibrateEmitter;
    public FMODUnity.StudioEventEmitter selectEmitter;
    public FMODUnity.StudioEventEmitter eerieEmitter;


    void Start()
    {
        phoneActive = false;
        _input.actions["Phone"].started += TogglePhone;

        foreach (TextMessage msg in texts)
        {
            if (msg.random)
                msg.time = UnityEngine.Random.Range(msg.minTime, msg.maxTime);    
        }

    }

    private void Update()
    {
        time += Time.deltaTime * timeScale;
        PhoneClock();
        CheckTimes();
    }

    public void TogglePhone(InputAction.CallbackContext context)
    {
        phoneActive = !phoneActive;
        if (phoneActive)
        {
            selectEmitter.Play();
        }
        animator.SetBool("On", phoneActive);
    }


    public void CheckTimes()
    {
        foreach (TextMessage msg in texts)
        {
            if (msg.time <= time && !msg.sent)
            {
                Debug.Log(msg.time);
                DisplayMessage(msg);
                msg.sent = true;
            }
        }
    }


    public void DisplayMessage(TextMessage msg)
    {
        GameObject messageBox = Instantiate(textPrefab, textContainer);
        messageBox.transform.SetSiblingIndex(0);
        TMP_Text message = messageBox.transform.GetChild(0).GetComponent<TMP_Text>();
        TMP_Text messageTime = messageBox.transform.GetChild(1).GetComponent<TMP_Text>();
        int hours = Mathf.FloorToInt(msg.time / 3600f);
        int minutes = Mathf.FloorToInt((msg.time - hours * 3600f) / 60f);

        if (hours <= 0)
            hours = 12;

        if (msg.sound)
        {
          eerieEmitter.Play();
        }    

        message.text = msg.text;
        messageTime.text = string.Format("{0:00}:{1:00}", hours, minutes);
        PhonePeek();
    }
    
    public void PhonePeek()
    {
        if(!phoneActive)
        {
            vibrateEmitter.Play();
            animator.SetBool("Peek", true);
            StartCoroutine(PeekTime());
        }
    }

    IEnumerator PeekTime()
    {
        yield return new WaitForSeconds(5f);
        animator.SetBool("Peek", false);
    }

    private void PhoneClock()
    {
        int hours = Mathf.FloorToInt(time / 3600f);
        int minutes = Mathf.FloorToInt((time - hours * 3600f) / 60f);

        if (hours == 0)
            hours = 12;

        clockText.text = string.Format("{0:00}:{1:00}", hours, minutes);
    }

    private void OnDisable()
    {
        _input.actions["Phone"].started -= TogglePhone;
    }

}
