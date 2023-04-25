using Sounds;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MascotAI : MonoBehaviour, MascotHearing
{
    //Navmesh
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask wallLayer;

    //Player
    [SerializeField] private Transform playerModel;
    private Vector3 playerLastPosition = Vector3.zero;
    private Vector3 playerPosition;

    //Waypoints
    [SerializeField] private Transform[] waypoints;
    private int currentWaypointIndex;

    //Editable Variables
    [SerializeField] private float startWaitTime = 2;
    [SerializeField] private float timeToRotate = 2;
    [SerializeField] private float walkSpeed = 2;
    [SerializeField] private float runSpeed = 4;
    [SerializeField] private float viewAngle = 90;
    [SerializeField] private float viewRadius = 15;
    [SerializeField] private float killRadius = 2;
    ///[SerializeField] float meshResolution = 1f;
    ///[SerializeField] float edgeDistance = 0.5f;
    ///[SerializeField] int edgeIterations = 4;

    //Uneditable Variables
    private float waitTime;
    private float rotateTime;
    private bool playerInRange;
    private bool playerNear;
    private bool isPatrol;
    private bool caughtPlayer;

    void Start()
    {
        playerPosition = Vector3.zero;
        isPatrol = true;
        caughtPlayer = false;
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

        if (!isPatrol)
        {
            Chasing();
        }
        else
        {
            Patrolling();
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
                    CaughtPlayer();
                }
            }
        }
    }

    void SpottedPlayer(Vector3 player)
    {
        agent.SetDestination(player);

        if (Vector3.Distance(transform.position, player) <= 0.3)
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
        transform.LookAt(playerModel);
        playerNear = false;
        playerLastPosition = Vector3.zero;

        if (!caughtPlayer)
        {
            Move(runSpeed);
            agent.SetDestination(playerPosition);
        }

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            if (waitTime <= 0 && !caughtPlayer && Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position) >= 6f)
            {
                isPatrol = true;
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

    void CaughtPlayer()
    {
        caughtPlayer = true;

        //activate jump scare kill animation
        Time.timeScale = 0;
    }

    private void KillPlayer()
    {
        transform.LookAt(playerModel);
        agent.speed = 100f;
        agent.SetDestination(playerModel.position);

        //activate jump scare kill animation
        Time.timeScale = 0;

    }

    public void RespondToSound(ObjectSound sound)
    {

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, killRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);
    }
}
