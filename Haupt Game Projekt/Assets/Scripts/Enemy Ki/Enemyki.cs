using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;




public class Enemyki : MonoBehaviour
{
    public Transform Player;
   
    [SerializeField]
    private Animator Animator = null;
    public float UpdateRate = 0.1f;
    Transform target;
    NavMeshAgent Agent;
    Animator anim;
  
    

    private const string iswalking = "iswalking";
    

    private Coroutine FollowCoroutine;

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
        

        OnStateChange += HandleStateChange;

    }
    private void Update()
    {
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

            /*if (oldState == EnemyState.Idle)
            {
                Agent.speed /= IdleMovespeedMultiplier;
            }*/

            switch (newState)
            {
                case EnemyState.Idle:
                   // FollowCoroutine = StartCoroutine(DoIdleMotion());
                    break;
                case EnemyState.Patrol:
                   // FollowCoroutine = StartCoroutine(DoPatrolMotion());
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
                Agent.SetDestination(Player.transform.position);
            }
            yield return Wait;
        }
    }
}
