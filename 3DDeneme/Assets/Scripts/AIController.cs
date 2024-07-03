using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    public enum AttackType
    {
        Melee,
        Shoot
    }
    
    public AttackType attackType = AttackType.Melee; 

    public NavMeshAgent navMeshAgent;
    public float startWaitTime = 4;
    public float timeToRotate = 2;
    public float speedWalk = 6;
    public float speedRun = 9;
    
    public float viewRadius = 1f;  
    public float shootViewRadius = 10f; 
    public float viewAngle = 360;
    public LayerMask playerMask;
    public LayerMask obstacleMask;
    
    private Transform[] waypoints;
    int m_CurrentWaypointIndex;
    
    Vector3 playerLastPosition = Vector3.zero;
    Vector3 m_PlayerPosition;
    
    float m_WaitTime;
    float m_TimeToRotate;
    bool m_PlayerInRange; 
    bool m_PlayerNear;
    bool m_IsPatrol;
    bool m_CaughtPlayer;
    
    public float attackRange = 2f; 
    public float shootRange = 10f; 
    public float attackCooldown = 1f; 
    private float lastAttackTime;
    
    [SerializeField]
    private ParticleSystem hitEffect;
    
    public GameObject projectile; 
    private bool alreadyAttacked;
    public float timeBetweenAttacks = 1f; 
    
    
    void Start()
    {
        m_PlayerPosition = Vector3.zero;
        m_IsPatrol = true;
        m_CaughtPlayer = false;
        m_PlayerInRange = false;
        m_WaitTime = startWaitTime;
        m_TimeToRotate = timeToRotate;
        m_CurrentWaypointIndex = 0;
        navMeshAgent = GetComponent<NavMeshAgent>();

        CreateWaypoints();
        navMeshAgent.isStopped = false;
        navMeshAgent.speed = speedWalk;
        navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
    }

    void Update()
    {
        EnvironmentView();

        if (!m_IsPatrol)
        {
            Chasing();
        }
        else
        {
            Patroling();
        }
        
        if (m_PlayerInRange)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, m_PlayerPosition);
            if (attackType == AttackType.Melee && distanceToPlayer <= attackRange)
            {
                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    Attack();
                    lastAttackTime = Time.time;
                }
            }
            else if (attackType == AttackType.Shoot && distanceToPlayer <= shootRange)
            {
                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    Shoot();
                    lastAttackTime = Time.time;
                }
            }
        }
    }

    private void Chasing()
    {
        m_PlayerNear = false;
        playerLastPosition = Vector3.zero;

        if (!m_CaughtPlayer)
        {
            Move(speedRun);
            navMeshAgent.SetDestination(m_PlayerPosition);
        }

        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            if (m_WaitTime <= 0 && !m_CaughtPlayer &&
                Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position) >= 6f)
            {
                m_IsPatrol = true;
                m_PlayerNear = false;
                Move(speedWalk);
                m_TimeToRotate = timeToRotate;
                m_WaitTime = startWaitTime;
                navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
            }
            else
            {
                if (Vector3.Distance(transform.position,
                        GameObject.FindGameObjectWithTag("Player").transform.position) >= 2.5f)
                {
                    Stop();
                    m_WaitTime -= Time.deltaTime;
                }
            }
        }
        
    }

    private void Patroling()
    {
        if (m_PlayerNear)
        {
            if (m_TimeToRotate <= 0)
            {
                Move(speedWalk);
                LookingPlayer(playerLastPosition);
            }
            else
            {
                Stop();
                m_TimeToRotate -= Time.deltaTime;
            }
        }
        else
        {
            m_PlayerNear = false;
            playerLastPosition = Vector3.zero;
            navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                if (m_WaitTime <= 0)
                {
                    NextPoint();
                    Move(speedWalk);
                    m_WaitTime = startWaitTime;
                }
                else
                {
                    Stop();
                    m_WaitTime -= Time.deltaTime;
                }
            }
        }
    }

    void Move(float speed)
    {
        navMeshAgent.isStopped = false;
        navMeshAgent.speed = speed;
    }

    void Stop()
    {
        navMeshAgent.isStopped = true;
        navMeshAgent.speed = 0;
    }

    public void NextPoint()
    {
        m_CurrentWaypointIndex = (m_CurrentWaypointIndex + 1) % waypoints.Length;
        navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
    }
    
    void CreateWaypoints()
    {
        
        float radius = 10f; 
        int numberOfWaypoints = 4; 
        waypoints = new Transform[numberOfWaypoints];

        for (int i = 0; i < numberOfWaypoints; i++)
        {
            
            float angle = i * Mathf.PI * 2 / numberOfWaypoints;
            Vector3 newPos = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
            GameObject waypoint = new GameObject("Waypoint" + (i + 1));
            waypoint.transform.position = transform.position + newPos;
            waypoints[i] = waypoint.transform;
        }
    }

    void LookingPlayer(Vector3 player)
    {
        navMeshAgent.SetDestination(player);
        if (Vector3.Distance(transform.position, player) <= 0.3)
        {
            if (m_WaitTime <= 0)
            {
                m_PlayerNear = false;
                Move(speedWalk);
                navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
                m_WaitTime = startWaitTime;
                m_TimeToRotate = timeToRotate;
            }
            else
            {
                Stop();
                m_WaitTime -= Time.deltaTime;
            }
        }
    }

    private void Attack()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            if (distanceToPlayer <= attackRange)
            {
                
                Actor playerActor = player.GetComponent<Actor>();
                if (playerActor != null)
                {
                    playerActor.TakeDamage(1); 
                    
                    if (hitEffect != null)
                    {
                        ParticleSystem effect = Instantiate(hitEffect, player.transform.position, Quaternion.identity);
                        effect.transform.SetParent(null);
                        effect.Play();

                        Destroy(effect.gameObject, effect.main.duration + effect.main.startLifetime.constant);
                    }
                }
                
            }
        }
    }

    private void Shoot()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            navMeshAgent.SetDestination(transform.position); 
            transform.LookAt(player.transform);

            if (!alreadyAttacked)
            {
                
                GameObject instantiatedProjectile = Instantiate(projectile, transform.position + transform.forward, Quaternion.identity);
                Rigidbody rb = instantiatedProjectile.GetComponent<Rigidbody>();
                rb.AddForce(transform.forward * 8f, ForceMode.Impulse);
                rb.AddForce(transform.up * 8f, ForceMode.Impulse);

                
                Projectile projectileScript = instantiatedProjectile.GetComponent<Projectile>();
                if (projectileScript != null)
                {
                    projectileScript.damage = 1; 
                    projectileScript.hitEffect = hitEffect; 
                }

                alreadyAttacked = true;
                Invoke(nameof(ResetAttack), timeBetweenAttacks);
            }
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    void EnvironmentView()
    {
        m_PlayerInRange = false;
        float currentViewRadius = attackType == AttackType.Melee ? viewRadius : shootViewRadius;
        
        Collider[] playerInRange = Physics.OverlapSphere(transform.position, currentViewRadius, playerMask);
        for (int i = 0; i < playerInRange.Length; i++)
        {
            Transform player = playerInRange[i].transform;
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToPlayer) < viewAngle / 2)
            {
                float dstToPlayer = Vector3.Distance(transform.position, player.position);
                if (!Physics.Raycast(transform.position, dirToPlayer, dstToPlayer, obstacleMask))
                {
                    m_PlayerInRange = true;
                    m_IsPatrol = false;
                }
                else
                {
                    m_PlayerInRange = false;
                }
            }

            if (Vector3.Distance(transform.position, player.position) > currentViewRadius)
            {
                m_PlayerInRange = false;
            }

            if (m_PlayerInRange)
            {
                m_PlayerPosition = player.transform.position;
            }
        }
    }
}
