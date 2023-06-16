using Sounds;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using StarterAssets;

public class MascotAI : MonoBehaviour, MascotHearing
{
    [SerializeField] private Transform mascotModel;
    [SerializeField] private Animator mascotAnimator;
    [SerializeField] private GameObject meter;
    [SerializeField] private Slider awarenessMeter;
    [SerializeField] private TextMeshProUGUI awarenessValueText;
    [SerializeField] private int awarenessMaxValue;
    [SerializeField] private int value;
    [SerializeField] private float runTickUp;
    [SerializeField] private float tickUp;
    [SerializeField] private float tickDown;

    //Player
    [Space(10)]
    [SerializeField] private Transform playerModel;
    [SerializeField] private FirstPersonController playerController;
    private Vector3 playerLastPosition = Vector3.zero;
    private Vector3 playerPosition;

    [Header("Nav Mesh")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask wallLayer;

    [Header("Mascot Movement")]
    [SerializeField] private float startWaitTime;
    [SerializeField] private float timeToRotate;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float huntSpeed;
    [SerializeField] private float killSpeed;
    [SerializeField] private float slowdownDuration;
    [SerializeField] private float slowdownMultiplier;

    [Header("Mascot Vision")]
    [Range(0, 360)]
    [SerializeField] public float viewAngle;
    [SerializeField] public float viewRadius;
    [SerializeField] public float killRadius;
    [SerializeField] public float awarenessRadius;

    //Waypoints
    [Space(10)]
    [SerializeField] private Transform[] waypoints;
    private int currentWaypointIndex;

    [Space(10)]
    [SerializeField] private GameObject _jumpscareObject;
    [SerializeField] private VideoPlayer _jumpscareVideo;

    //Uneditable Variables
    private float waitTime;
    private float rotateTime;
    [SerializeField] private bool playerInRange;
    private bool playerNear;
    private bool isPatrol;
    public bool isChasing { get ; private set; }
    private bool isDistracted;
    public bool isHunting { get; private set; }
    private bool killPlayer;
    private bool caughtPlayer;
    private bool reachedObject;
    private bool reachedPosition;
    private bool stopTimer;
    private float runTimerUp;
    protected float timerUp;
    protected float timerDown;

    //audio
    public FMODUnity.StudioEventEmitter chaseEmitter;
    public FMODUnity.StudioEventEmitter meter50Emitter;
    public FMODUnity.StudioEventEmitter walkEmitter;
    public FMODUnity.StudioEventEmitter runEmitter;
    public FMODUnity.StudioEventEmitter hitEmitter;
    [SerializeField] private FMODUnity.StudioEventEmitter _ambientNoiseEmitter;

    public Hiding hide;

    void Start()
    {
        _jumpscareVideo.loopPointReached += LoadGameOver;
        awarenessMeter.maxValue = awarenessMaxValue;
        awarenessValueText.text = value.ToString();

        playerPosition = Vector3.zero;
        isPatrol = true;
        isDistracted = false;
        isChasing = false;
        isHunting = false;
        killPlayer = false;
        caughtPlayer = false;
        reachedObject = false;
        reachedPosition = false;
        playerInRange = false;
        stopTimer = false;
        waitTime = startWaitTime;
        rotateTime = timeToRotate;

        currentWaypointIndex = Random.Range(0, waypoints.Length);

        agent = GetComponent<NavMeshAgent>();
        agent.isStopped = false;
        agent.speed = walkSpeed;
        agent.SetDestination(waypoints[currentWaypointIndex].position);
    }

    private void Update()
    {
        DangerMeter();

        if (Phone.isSunrise) // Added sunrise bool here -JOSH
        {
            isPatrol = false;
            isDistracted = false;
            isHunting = false;
            playerInRange = true;
        }

      

        if ((hide.isHidden == true) && (isPatrol == true))
        {
            ///the mascot can't see the player hiding while in patrol mode; 
        }
        else
        {
            MascotView();
        }

        if (!isPatrol && (isDistracted == false) && (isHunting == false))
        {
            Chasing();
        }
        else if ((isDistracted == false) && (isHunting == false))
        {
            Patrolling();

            if ((Vector3.Distance(transform.position, playerModel.position) <= awarenessRadius) && (hide.isHidden == false))
            {
                float mascotRotation = transform.localRotation.eulerAngles.y;
                transform.Rotate(0f, 180f, 0f);
            }
        }
        else if ((isDistracted == true) && (isChasing == false) && (isHunting == false))
        {
            Distracted();
        }
        else if (isHunting == true)
        {
            Hunting();
        }

        if ((isChasing == true) || (isHunting == true))
        {
            mascotAnimator.SetTrigger("Run");
        }

        if ((isDistracted == true) || (isPatrol == true))
        {
            mascotAnimator.SetTrigger("Walk");
        }

        /*if (isPatrol == true && !agent.hasPath && agent.pathStatus == NavMeshPathStatus.PathComplete)
        {
            // mascot stuck
            agent.enabled = false;
            Debug.Log("mascot stuck: navmesh disabled");

            // mascot will HOPEFULLY start moving again
            agent.enabled = true;
            Debug.Log("mascot unstuck: navmesh re-enabled");
        }*/
    }

    void OnDestroy()
    {
        chaseEmitter.Stop();
        meter50Emitter.Stop();
    }

    void Move(float speed)
    {
        agent.isStopped = false;
        agent.speed = speed;

        if (speed == walkSpeed)
        {
            walkEmitter.Play();
        }
        else
        {
            runEmitter.Play();
        }

        runEmitter.SetParameter("isSlowed", 0);
    }

    void Stop()
    {
        agent.isStopped = true;
        agent.speed = 0;

        walkEmitter.Stop();
        runEmitter.Stop();
    }

    private void NextPoint()
    {
        currentWaypointIndex = (currentWaypointIndex + Random.Range(0, waypoints.Length - 1)) % waypoints.Length;
        agent.SetDestination(waypoints[currentWaypointIndex].position);
    }

    void MascotView()
    {
        Collider[] playerInView = Physics.OverlapSphere(transform.position, viewRadius, playerLayer);

        for (int i = 0; i < playerInView.Length; i++)
        {
            Transform player = playerInView[i].transform;
            Vector3 directionToPlayer = (player.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToPlayer) < viewAngle / 2 || Phone.isSunrise) // Added sunrise bool here -JOSH
            {
                float distanceToPlayer = Vector3.Distance(transform.position, player.position);

                if (!Physics.Raycast(transform.position, directionToPlayer, distanceToPlayer, wallLayer) || Phone.isSunrise) // Added sunrise bool here -JOSH
                {
                    meter.SetActive(true);

                    playerInRange = true;
                    isPatrol = false;

                    if (Vector3.Distance(transform.position, player.position) <= killRadius)
                    {
                        isChasing = false;
                        isPatrol = false;
                        isHunting = false;
                        isDistracted = false;
                        value = awarenessMaxValue;
                        awarenessMeter.value = value;
                        awarenessValueText.text = value.ToString();
                        transform.LookAt(playerModel);
                        caughtPlayer = true;
                        killPlayer = true;
                        stopTimer = true;
                        mascotAnimator.SetBool("Idling", true);
                        Stop();
                        PlayJumpscare();
                    }
                }
                else
                {
                    playerInRange = false;

                    if (isPatrol == true)
                    {
                        Move(walkSpeed);
                    }

                    if (isChasing == true)
                    {
                        Move(runSpeed);
                    }

                    if (isHunting == true)
                    {
                        Move(huntSpeed);
                    }

                    agent.SetDestination(playerLastPosition);
                    Debug.Log("BEHIND A WALL");
                }
            }

            if (Vector3.Distance(transform.position, player.position) > viewRadius && !Phone.isSunrise)
            {
                playerInRange = false;
            }

            if (playerInRange == true)
            {
                playerPosition = player.transform.position;
            }
        }
    }

    void SpottedPlayer(Vector3 player)
    {
        agent.SetDestination(player);

        if (Vector3.Distance(transform.position, player) <= 0)
        {
            if (waitTime <= 0)
            {
                playerNear = false;
                Move(walkSpeed);
                agent.SetDestination(waypoints[currentWaypointIndex].position);
                waitTime = startWaitTime;
                rotateTime = timeToRotate;
            }
            else
            {
                mascotAnimator.SetTrigger("Idle");
                Stop();
                waitTime -= Time.deltaTime;
            }
        }
    }

    private void Patrolling()
    {
        if (playerNear == true)
        {
            if (rotateTime <= 0)
            {
                Move(walkSpeed);
                SpottedPlayer(playerLastPosition);
            }
            else
            {
                mascotAnimator.SetTrigger("Idle");
                Stop();
                rotateTime -= Time.fixedDeltaTime;
            }
        }
        else
        {
            playerNear = false;
            playerLastPosition = Vector3.zero;
            agent.SetDestination(waypoints[currentWaypointIndex].position);

            Debug.Log(agent.remainingDistance);
            Debug.Log(agent.stoppingDistance);

            if ((agent.remainingDistance <= agent.stoppingDistance) && (agent.remainingDistance != 0))
            {
                if(waitTime <= 0)
                {
                    NextPoint();
                    Move(walkSpeed);
                    waitTime = startWaitTime;
                }
                else
                {
                    mascotAnimator.SetTrigger("Idle");
                    Stop();
                    waitTime -= Time.fixedDeltaTime;
                }
            }
            else if (agent.remainingDistance == 0)
            {
                NextPoint();
                Move(walkSpeed);
                waitTime = startWaitTime;
            }
        }
    }

    private void Chasing()
    {
        isChasing = true;
        chaseEmitter.SetParameter("Chasing", 1f);
        transform.LookAt(playerModel);
        playerNear = false;
        playerLastPosition = Vector3.zero;

        if (!chaseEmitter.IsPlaying())
        {
            chaseEmitter.Play(); // plays chase sound, can be moved to a different place if we need it somewhere else
        }

        if (!killPlayer)
        {
            Move(runSpeed);
            agent.SetDestination(playerPosition);
        }

        if (agent.remainingDistance <= agent.stoppingDistance || !Phone.isSunrise) // Added sunrise bool here -JOSH
        {
            if (waitTime <= 0 && !killPlayer && Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position) >= 6f)
            {
                isPatrol = true;
                isChasing = false;
                chaseEmitter.SetParameter("Chasing", 0);
                playerNear = false;
                Move(walkSpeed);
                rotateTime = timeToRotate;
                waitTime = startWaitTime;
                agent.SetDestination(waypoints[currentWaypointIndex].position);
            }
            else
            {
                if (Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position) >= 2f)
                {
                    mascotAnimator.SetTrigger("Idle");
                    Stop();
                    waitTime -= Time.deltaTime;
                }
            }
        }
    }

    private void Hunting()
    {
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            if (waitTime <= 0)
            {
                isPatrol = true;
                isHunting = false;
                playerNear = false;
                reachedPosition = true;
                Move(walkSpeed);
                rotateTime = timeToRotate;
                waitTime = startWaitTime;
                agent.SetDestination(waypoints[currentWaypointIndex].position);
            }
            else
            {
                mascotAnimator.SetTrigger("Idle");
                Stop();
                waitTime -= Time.fixedDeltaTime;
            }
        }
    }

    void DangerMeter()
    {
        // increases danger meter when the player is running
        if ((stopTimer == false) && (playerController.isRunning == true) && (isChasing == false))
        {
            runTimerUp += Time.deltaTime;
        }

        // danger meter ticks up
        if (runTimerUp >= runTickUp)
        {
            if (value < awarenessMaxValue)
            {
                runTimerUp = 0f;
                value++;
                awarenessMeter.value = value;
                awarenessValueText.text = value.ToString();
            }
        }

        // increases danger meter when the mascot is chasing the player
        if ((stopTimer == false) && (playerInRange == true))
        {
            timerUp += Time.deltaTime;
        }
        
        // danger meter ticks up
        if (timerUp >= tickUp)
        {
            if (value < awarenessMaxValue)
            {
                timerUp = 0f;
                value++;
                awarenessMeter.value = value;
                awarenessValueText.text = value.ToString();
            }
        }

        // decreases danger meter when the mascot is not chasing the player
        if ((stopTimer == false) && (playerInRange == false))
        {
            timerDown += Time.deltaTime;
        }

        // danger meter ticks down
        if (timerDown >= tickDown)
        {
            if (value > 0)
            {
                timerDown = 0f;
                value--;
                awarenessMeter.value = value;
                awarenessValueText.text = value.ToString();
            }
        }

        if ((value == awarenessMaxValue / 2) && (value < awarenessMaxValue)) // half
        {
            if (!meter50Emitter.IsPlaying())
            {
                meter50Emitter.Play();
            }

            isPatrol = false;
            playerNear = false;

            if (reachedPosition == false)
            {
                isHunting = true;
                Move(huntSpeed);
                agent.SetDestination(playerLastPosition);
                agent.isStopped = false;
            }
        }

        if ((value >= awarenessMaxValue) && (caughtPlayer == false))
        {
            killPlayer = true;
            KillPlayer();
        }
    }

    private void KillPlayer()
    {
        if (killPlayer == true)
        {
            stopTimer = true;
            isPatrol = false;
            isHunting = false;
            isDistracted = false;
            transform.LookAt(playerModel);
            runSpeed = killSpeed;
            Move(runSpeed);
            agent.isStopped = false;
            agent.SetDestination(playerModel.position);

            if (Vector3.Distance(transform.position, playerModel.position) <= killRadius)
            {
                mascotAnimator.SetBool("Idling", true);
                playerModel.transform.LookAt(mascotModel);
                Stop();
                PlayJumpscare();
            }
        }
    }

    public void RespondToSound(ObjectSound sound)
    {
        if (isChasing == false)
        {
            isDistracted = true;
            isPatrol = false;
            playerNear = false;
            Debug.Log(name + " responding to sound at " + sound.position);

            if (reachedObject == false)
            {
                Move(walkSpeed);
                agent.SetDestination(sound.position);
                agent.isStopped = false;
            }
        }
    }

    private void Distracted()
    {
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            if (waitTime <= 0)
            {
                Debug.Log(name + " reached sound");

                isPatrol = true;
                isDistracted = false;
                playerNear = false;
                reachedObject = true;
                Move(walkSpeed);
                rotateTime = timeToRotate;
                waitTime = startWaitTime;
                agent.SetDestination(waypoints[currentWaypointIndex].position);
                reachedObject = false;
            }
            else
            {
                mascotAnimator.SetTrigger("Idle");
                Stop();
                waitTime -= Time.fixedDeltaTime;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Throwable")
        {
            GameObject throwable = GameObject.FindWithTag("Throwable");
            Rigidbody rigidbody = throwable.GetComponent<Rigidbody>();

            if (rigidbody.velocity.magnitude > 1)
            {
                Debug.Log("SLOWDOWN ACTIVATED");

                walkSpeed *= slowdownMultiplier;
                runSpeed *= slowdownMultiplier;
                hitEmitter.Play();
                runEmitter.SetParameter("isSlowed", 1);
                StartCoroutine(Slowdown());
            }
            else
            {
                return;
            }
        }
        else
        {
            return;
        }
    }

    IEnumerator Slowdown()
    {
        yield return new WaitForSeconds(slowdownDuration);
        walkSpeed /= slowdownMultiplier;
        runSpeed /= slowdownMultiplier;
    }


    private void PlayJumpscare()
    {
        Destroy(_ambientNoiseEmitter.gameObject);
        meter.SetActive(false);
        _jumpscareObject.SetActive(true);  // Play on Awake is set to true, so should automatically work - Diego
    }


    private void LoadGameOver(VideoPlayer vp)
    {
        GameObject.Find("SaveBetweenScenes").GetComponent<SaveBetweenScenes>().PlayerWon = false;
        SceneManager.LoadScene("Game Over");
    }

    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    //COMMENT OUT WHEN BULDING GAME//
    /*
    private void OnDrawGizmos()
    {
        Handles.color = Color.white;
        Handles.DrawWireArc(transform.position, Vector3.up, Vector3.forward, 360, viewRadius);

        Vector3 viewAngle1 = DirectionFromAngle(transform.eulerAngles.y, -viewAngle / 2);
        Vector3 viewAngle2 = DirectionFromAngle(transform.eulerAngles.y, viewAngle / 2);

        Handles.color = Color.yellow;
        Handles.DrawLine(transform.position, transform.position + viewAngle1 * viewRadius);
        Handles.DrawLine(transform.position, transform.position + viewAngle2 * viewRadius);

        Handles.color = Color.red;
        Handles.DrawWireArc(transform.position, Vector3.up, Vector3.forward, 360, killRadius);

        Handles.color = Color.blue;
        Handles.DrawWireArc(transform.position, Vector3.up, Vector3.forward, 360, awarenessRadius);
    }
    */
    //COMMENT OUT WHEN BULDING GAME//
}
