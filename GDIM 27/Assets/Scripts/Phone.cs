using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System;
using System.Security.Cryptography;

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
        public bool isPreloaded; // is it a "pre-loaded" msg - Diego
        public bool playEerieSfx; // does the text play the eerie sfx
    }

    public bool startTimer;
    [SerializeField] private PlayerInput _input;
    [SerializeField] private Animator animator;
    [SerializeField] private TMP_Text clockText;
    [SerializeField] private float time;
    [SerializeField] private float timeScale; // how fast time should pass in game
    [SerializeField] private float sunriseTime;
    
    [SerializeField] private Transform textContainer;
    [SerializeField] private GameObject textPrefab;
    [SerializeField] private Color sunriseColor;
    [SerializeField] private Light envLight; //environment Light 
    
    public bool recievedText;
    private bool phoneActive;
    public static bool isSunrise;

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

            if (msg.isPreloaded) // Display all "pre-loaded" msg's - Diego
            {
                DisplayMessage(msg);
                msg.sent = true;
            }
        }
        
        

    }

    private void Update()
    {
        if (!startTimer) return;
        time += Time.deltaTime * timeScale;
        PhoneClock();
        CheckTimes();
        CheckSunrise();
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

    public void CheckSunrise()
    {
        if (time >= sunriseTime)
        {
            envLight.color = sunriseColor;
            isSunrise = true;
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

        if (msg.playEerieSfx)
        {
          eerieEmitter.Play();
        }    

        message.text = msg.text;
        messageTime.text = string.Format("{0:00}:{1:00}", hours, minutes);

        if (!msg.isPreloaded)  // Ensures that all DisplayMessage() calls in Start for "pre-loaded" msg's don't activate "Peek" anim + vibrate sfx - Diego
        {
            PhonePeek();
        }
    }
    
    public void PhonePeek()
    {
        vibrateEmitter.Play(); // moved to outside if
        
        if(!phoneActive)
        {
            
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
        if (_input == null)
        {
            return;
        }

        _input.actions["Phone"].started -= TogglePhone;
    }

}
