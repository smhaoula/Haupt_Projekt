using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;




public class Enemyki : MonoBehaviour
{
    public Transform Player;
    public NavMeshTriangulation Triangulation;
    public EnemyLineOfSightChecker LineOfSightChecker;
    public FireChecker fireChecker;
    [SerializeField]
    private Animator Animator = null;
    public float UpdateRate = 0.1f;
    Transform target;
    NavMeshAgent Agent;
    Animator anim;

    public HealthBar healthBar;
    public int Health = 100;
    public int currentHealth;
    public bool isDefeated;


    public Vector3[] Waypoints = new Vector3[4];
    [SerializeField]
    private int WaypointIndex = 0;


    private const string iswalking = "iswalking";
    private const string isattacking = "isattacking";


    private Coroutine FollowCoroutine;
    public float IdleLocationRadius = 4f;
    public float IdleMovespeedMultiplier = 0.5f;

    public EnemyState DefaultState;
    private EnemyState _state;
    public EnemyState State
    {
        get
        {
            return _state;
        }
        set
        {
            OnStateChange?.Invoke(_state, value);
            _state = value;
        }
    }

    public delegate void StateChangeEvent(EnemyState oldState, EnemyState newState);
    public StateChangeEvent OnStateChange;
    private void Awake()
    {
        
        Agent = GetComponent<NavMeshAgent>();
        Triangulation = NavMesh.CalculateTriangulation();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        anim = GetComponent<Animator>();


        OnStateChange += HandleStateChange;

        LineOfSightChecker.onGainSight += HandleGainSight;
        LineOfSightChecker.OnLoseSight += HandleLoseSight;

        fireChecker.OnGainSight += HandleFireSight;
        fireChecker.OnLoseSight += HandleLoseFireSight;


        OnStateChange?.Invoke(EnemyState.Spawn, DefaultState);


    }

    private void Start()
    {
        currentHealth = Health;
        healthBar.SetMaxHealth(Health);
        isDefeated = false;
        Triangulation = NavMesh.CalculateTriangulation();
        Player = GameObject.FindWithTag("Player").transform;
        Spawn();
    }

    public void TakeDamage(int damage){
        if (!isDefeated)
        {
            if (currentHealth <= 0)
            {
                ScoreScript.scoreValue += 10;
                ScoreScript.currentScore +=10;
                isDefeated = true;
                anim.SetBool("defeated", true);
                StartCoroutine(DeathAnimation());
            }
            else {

                anim.SetBool("gethit", true);
                StartCoroutine(HitAnimation());

                currentHealth = currentHealth - damage;
                healthBar.SetHealth(currentHealth);
                if (currentHealth <= currentHealth / 2) {
                    HandleHealthSight();
                   
                
                
                }
            }
        }
        
    }
    IEnumerator HitAnimation()
    {
        yield return new WaitForSeconds(0.5f);
        anim.SetBool("gethit", false);

    }

    IEnumerator DeathAnimation(){
        yield return new WaitForSeconds(2f);
        Destroy(gameObject, 1f);
    }

    private void HandleHealthSight()
    {
        State = EnemyState.Patrol;
    }

    private void HandleLoseHealthSight()
    {
        State = DefaultState;
    }
    private void HandleFireSight()
    {
        State = EnemyState.Idle;
    }

    private void HandleLoseFireSight()
    {
        State = DefaultState;
    }
    private void HandleGainSight()
    {
        State = EnemyState.Chase;
    }

    private void HandleLoseSight()
    {
        State = DefaultState;
    }
    public void Spawn()
    {
        for (int i = 0; i < Waypoints.Length; i++)
        {
            NavMeshHit Hit;
            if (NavMesh.SamplePosition(Triangulation.vertices[Random.Range(0, Triangulation.vertices.Length)], out Hit, 2f, Agent.areaMask))
            {
                Waypoints[i] = Hit.position;
            }
            else
            {
                Debug.LogError("Unable to find position for navmesh near Triangulation vertex!");
            }
        }
        OnStateChange?.Invoke(EnemyState.Spawn, DefaultState);
    }
 



    private void Update()
    {
        Player = GameObject.FindWithTag("Player").transform;
        Animator.SetBool(iswalking, Agent.velocity.magnitude > 0.01f);
        
        /* Agent.updatePosition = true;
         Agent.SetDestination(Player.transform.position);
         Animator.SetBool("iswalking", true);*/
    }

   
    private void OnDisable()
    {
        _state = DefaultState; // use _state to avoid triggering OnStateChange when recycling object in the pool
    }
    private void HandleStateChange(EnemyState oldState, EnemyState newState)
    {
        if (oldState != newState)
        {
            if (FollowCoroutine != null)
            {
                StopCoroutine(FollowCoroutine);
            }

            if (oldState == EnemyState.Idle)
            {
                Agent.speed /= IdleMovespeedMultiplier;
            }

            switch (newState)
            {
                case EnemyState.Idle:
                     FollowCoroutine = StartCoroutine(DoIdleMotion());
                    break;
                case EnemyState.Patrol:
                     FollowCoroutine = StartCoroutine(DoPatrolMotion());
                    break;
                case EnemyState.Chase:
                    FollowCoroutine = StartCoroutine(FollowTarget());
                    break;
            }
        }
    }
    private IEnumerator FollowTarget()
    {
        WaitForSeconds Wait = new WaitForSeconds(UpdateRate);

        while (true)
        {
            if (Agent.enabled)
            {
                float distance = Vector3.Distance(transform.position, target.position);
                if (distance > 2)
                {
                    Agent.isStopped = false;
                    anim.SetBool("isattacking", false);
                    Player = GameObject.FindWithTag("Player").transform;
                    Agent.SetDestination(Player.transform.position);

                }
                else {
                   Agent.isStopped = true;
                   
                    anim.SetBool("isattacking", true); }
                   
                
            }
            yield return Wait;
        }
    }

    private IEnumerator DoIdleMotion()
    {
        WaitForSeconds Wait = new WaitForSeconds(UpdateRate);

        Agent.speed *= IdleMovespeedMultiplier;

        while (true)
        {
            if (!Agent.enabled || !Agent.isOnNavMesh)
            {
                yield return Wait;
            }
            else if (Agent.remainingDistance <= Agent.stoppingDistance)
            {
                Vector2 point = Random.insideUnitCircle * IdleLocationRadius;
                NavMeshHit hit;

                if (NavMesh.SamplePosition(Agent.transform.position + new Vector3(point.x, 0, point.y), out hit, 2f, Agent.areaMask))
                {
                    Agent.SetDestination(hit.position);
                }
            }

            yield return Wait;
        }
    }
    private IEnumerator DoPatrolMotion()
    {
        WaitForSeconds Wait = new WaitForSeconds(UpdateRate);

        yield return new WaitUntil(() => Agent.enabled && Agent.isOnNavMesh);
        Agent.SetDestination(Waypoints[WaypointIndex]);

        while (true)
        {
            if (Agent.isOnNavMesh && Agent.enabled && Agent.remainingDistance <= Agent.stoppingDistance)
            {
                WaypointIndex++;

                if (WaypointIndex >= Waypoints.Length)
                {
                    WaypointIndex = 0;
                }

                Agent.SetDestination(Waypoints[WaypointIndex]);
            }

            yield return Wait;
        }
    }
    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < Waypoints.Length; i++)
        {
            Gizmos.DrawWireSphere(Waypoints[i], 0.25f);
            if (i + 1 < Waypoints.Length)
            {
                Gizmos.DrawLine(Waypoints[i], Waypoints[i + 1]);
            }
            else
            {
                Gizmos.DrawLine(Waypoints[i], Waypoints[0]);
            }
        }
    }
}
