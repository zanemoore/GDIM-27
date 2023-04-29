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
    [SerializeField] private Slider awarenessMeter;
    [SerializeField] private TextMeshProUGUI awarenessValueText;
    [SerializeField] private int awarenessMaxValue;
    [SerializeField] private int value;

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
    [SerializeField] private float startWaitTime = 1;
    [SerializeField] private float timeToRotate = 2;
    [SerializeField] private float walkSpeed = 2;
    [SerializeField] private float runSpeed = 4;
    [SerializeField] private float killSpeed = 16;
    [SerializeField] private float slowdownDuration = 2;
    [SerializeField] private float slowdownMultiplier = 0.5f;

    [Header("Mascot Vision")]
    [Range(0, 360)]
    [SerializeField] public float viewAngle = 90;
    [SerializeField] public float viewRadius = 15;
    [SerializeField] public float killRadius = 2;
    [SerializeField] public float awarenessRadius = 5;

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
    private bool killPlayer;
    private bool reachedObject;
    private bool stopTimer = false;
    private int tickUp = 1;
    private int tickDown = 2;
    protected float timerUp;
    protected float timerDown;

    [HideInInspector]
    public AwarenessMeter meter;

    void Start()
    {
        awarenessMeter.maxValue = awarenessMaxValue;
        awarenessValueText.text = value.ToString();

        playerPosition = Vector3.zero;
        isPatrol = true;
        isDistracted = false;
        isChasing = false;
        killPlayer = false;
        reachedObject = false;
        playerInRange = false;
        waitTime = startWaitTime;
        rotateTime = timeToRotate;

        currentWaypointIndex = 0;

        agent = GetComponent<NavMeshAgent>();
        agent.isStopped = false;
        agent.speed = walkSpeed;
        agent.SetDestination(waypoints[currentWaypointIndex].position);
    }

    private void Update()
    {
        MascotView();
        AwarenessMeter();

        if (!isPatrol && (isDistracted == false))
        {
            Chasing();
        }
        else if (isDistracted == false)
        {
            Patrolling();

            if (Vector3.Distance(transform.position, playerModel.position) <= awarenessRadius)
            {
                float mascotRotation = transform.localRotation.eulerAngles.y;
                transform.Rotate(0f, 180f, 0f);
            }
        }
        else if (isDistracted == true)
        {
            Distracted();
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
        currentWaypointIndex = Random.Range(0, waypoints.Length);
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
                }
                else
                {
                    playerInRange = false;
                }
            }

            if (Vector3.Distance(transform.position, player.position) > viewRadius)
            {
                playerInRange = false;
            }

            if (playerInRange)
            {
                playerPosition = player.transform.position;

                if (Vector3.Distance(transform.position, player.position) <= killRadius)
                {
                    killPlayer = true;
                    transform.LookAt(playerModel);
                    //activate jump scare kill animation
                }
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
                Stop();
                waitTime -= Time.deltaTime;
            }
        }
    }

    private void Patrolling()
    {
        if (playerNear)
        {
            if (rotateTime <= 0)
            {
                Move(walkSpeed);
                SpottedPlayer(playerLastPosition);
            }
            else
            {
                Stop();
                rotateTime -= Time.deltaTime;
            }
        }
        else
        {
            playerNear = false;
            playerLastPosition = Vector3.zero;
            agent.SetDestination(waypoints[currentWaypointIndex].position);

            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if(waitTime <= 0)
                {
                    NextPoint();
                    Move(walkSpeed);
                    waitTime = startWaitTime;
                }
                else
                {
                    Stop();
                    waitTime -= Time.deltaTime;
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
                if (Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position) >= 2.5f)
                {
                    Stop();
                    waitTime -= Time.deltaTime;
                }
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
            timerUp = 0f;
            value++;
            awarenessMeter.value = value;
            awarenessValueText.text = value.ToString();
        }

        if (stopTimer == false)
        {
            timerDown += Time.deltaTime;
        }

        if ((timerDown >= tickDown) && (playerInRange == false))
        {
            if (stopTimer == true)
            {
                return;
            }
            else
            {
                timerDown = 0f;
                value--;
                awarenessMeter.value = value;
                awarenessValueText.text = value.ToString();
            }
        }

        if (value <= 0)
        {
            stopTimer = true;
        }

        if ((value >= awarenessMaxValue / 2) && (value < awarenessMaxValue))
        {
            //Move(walkSpeed);
            //agent.isStopped = false;
            //agent.SetDestination(playerLastPosition);
        }

        if (value >= awarenessMaxValue)
        {
            //killPlayer = true;
            //KillPlayer();
        }
    }

    private void KillPlayer()
    {
        if (killPlayer == true)
        {
            stopTimer = true;
            isPatrol = false;
            transform.LookAt(playerModel);
            runSpeed = killSpeed;
            Move(runSpeed);
            agent.isStopped = false;
            agent.SetDestination(playerModel.position);

            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (waitTime <= 0)
                {
                    //activate jump scare kill animation
                    killPlayer = false;
                }
                else
                {
                    
                }
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
            }
            else
            {
                Stop();
                waitTime -= Time.deltaTime;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Throwable")
        {
            StartCoroutine(Slowdown());
        }
    }

    IEnumerator Slowdown()
    {
        float walk = walkSpeed;
        float run = runSpeed;
        walkSpeed *= slowdownMultiplier;
        runSpeed *= slowdownMultiplier;
        yield return new WaitForSeconds(slowdownDuration);
        walkSpeed = walk;
        runSpeed = run;
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
