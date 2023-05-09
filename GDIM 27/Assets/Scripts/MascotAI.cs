using Sounds;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class MascotAI : MonoBehaviour, MascotHearing
{
    [SerializeField] private Transform mascotModel;
    [SerializeField] private Animator mascotAnimator;
    [SerializeField] private Slider awarenessMeter;
    [SerializeField] private TextMeshProUGUI awarenessValueText;
    [SerializeField] private int awarenessMaxValue;
    [SerializeField] private int value;
    [SerializeField] private int tickUp;
    [SerializeField] private int tickDown;

    //Player
    [Space(10)]
    [SerializeField] private Transform playerModel;
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

    //Uneditable Variables
    private float waitTime;
    private float rotateTime;
    private bool playerInRange;
    private bool playerNear;
    private bool isPatrol;
    private bool isChasing;
    private bool isDistracted;
    private bool isHunting;
    private bool killPlayer;
    private bool caughtPlayer;
    private bool reachedObject;
    private bool reachedPosition;
    private bool stopTimer;
    protected float timerUp;
    protected float timerDown;

    [HideInInspector]
    public AwarenessMeter meter;

    //audio
    public FMODUnity.StudioEventEmitter chaseEmitter;
    public FMODUnity.StudioEventEmitter meter25Emitter;
    public FMODUnity.StudioEventEmitter meter50Emitter;
    public FMODUnity.StudioEventEmitter meter75Emitter;
    
    public Hiding hide;

    void Start()
    {
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
        AwarenessMeter();

        if ((hide.isHidden == true) && (isPatrol == true))
        {
            ///player hidden while mascot is patrolling
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
        else if (isDistracted == true)
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
    }

    void Move(float speed)
    {
        agent.isStopped = false;
        agent.speed = speed;
    }

    void Stop()
    {
        agent.isStopped = true;
        agent.speed = 0;
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

            if (Vector3.Angle(transform.forward, directionToPlayer) < viewAngle / 2)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, player.position);

                if (!Physics.Raycast(transform.position, directionToPlayer, distanceToPlayer, wallLayer))
                {
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
                        //activate jump scare kill animation
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
                    //Debug.Log("BEHIND A WALL");
                }

                
            }

            if (Vector3.Distance(transform.position, player.position) > viewRadius)
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
        }
    }

    private void Chasing()
    {
        isChasing = true;
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

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            if (waitTime <= 0 && !killPlayer && Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position) >= 6f)
            {
                isPatrol = true;
                isChasing = false;
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

    void AwarenessMeter()
    {
        if (stopTimer == false)
        {
            timerUp += Time.deltaTime;
        }
        

        if ((timerUp >= tickUp) && (playerInRange == true))
        {
            if (value < awarenessMaxValue)
            {
                timerUp = 0f;
                value++;
                awarenessMeter.value = value;
                awarenessValueText.text = value.ToString();
            }
        }

        if (stopTimer == false)
        {
            timerDown += Time.deltaTime;
        }

        if ((timerDown >= tickDown) && (playerInRange == false))
        {
            if (value > 0)
            {
                timerDown = 0f;
                value--;
                awarenessMeter.value = value;
                awarenessValueText.text = value.ToString();
            }
        }

        if (value == awarenessMaxValue / 4) // a quarter
        {
            if (!meter25Emitter.IsPlaying())
            {
                meter25Emitter.Play();
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

        if (value == awarenessMaxValue * 3 / 4) // 3 quarters
        {
            if (!meter75Emitter.IsPlaying())
            {
                meter75Emitter.Play();
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
                //activate jump scare kill animation?
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

            //Debug.Log(rigidbody.velocity.magnitude);


            if (rigidbody.velocity.magnitude > 1)
            {
                Debug.Log("SLOWDOWN ACTIVATED");

                walkSpeed *= slowdownMultiplier;
                runSpeed *= slowdownMultiplier;
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

    //COMMENT OUT WHEN BULDING GAME//
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

    //COMMENT OUT WHEN BULDING GAME//
    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
