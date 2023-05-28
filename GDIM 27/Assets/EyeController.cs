using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeController : MonoBehaviour
{
    // Start is called before the first frame update

    public Animator eyeAnimator;
    public MascotAI AI;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        eyeAnimator.SetBool("Open", AI.isChasing || AI.isHunting);
    }
}
