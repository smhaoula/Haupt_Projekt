using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemytest : MonoBehaviour
{
    Transform target;
    Animator anim;

    NavMeshAgent agent;
    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        float distance = Vector3.Distance(transform.position, target.position);
        if (distance > 1.5)
        {
            agent.updatePosition = true;

            agent.SetDestination(target.position);
            anim.SetBool("iswalking", true);
            anim.SetBool("isattaking", false);
        }
        else
        {
            agent.updatePosition = false;
            anim.SetBool("iswalking", false);
            anim.SetBool("isattaking", true);

        }
    }


}
