using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class EnemyLineOfSightChecker : MonoBehaviour
{
    public SphereCollider Collider;
    public float FieldOfView = 90f;
    public LayerMask LineOfSightLayers;


    public SignEvent onGainSight;
    public delegate void SignEvent();
    public SignEvent OnLoseSight;

    private Coroutine CheckForLineOfSightCoroutine;
   
    public FireChecker fireChecker;
    private bool isactive;


    private void Awake()
    {
        fireChecker.onCoverChange += setbool;
        Collider = GetComponent<SphereCollider>();
    }

   
    void setbool(bool var) {
        isactive = !var;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (isactive)
        {
            PlayerMovement player;
            if (other.TryGetComponent<PlayerMovement>(out player))
            {
                if (!CheckLineOfSight(player))
                {
                    CheckForLineOfSightCoroutine = StartCoroutine(CheckForLineOfSight(player));
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isactive)
        {
            PlayerMovement player;
            if (other.TryGetComponent<PlayerMovement>(out player))
            {
                OnLoseSight?.Invoke();
                if (CheckForLineOfSightCoroutine != null)
                {
                    StopCoroutine(CheckForLineOfSightCoroutine);
                }
            }
        }
    }

    private bool CheckLineOfSight(PlayerMovement player)
    {
        Vector3 Direction = (player.transform.position - transform.position).normalized;
        float DotProduct = Vector3.Dot(transform.forward, Direction);
        if (DotProduct >= Mathf.Cos(FieldOfView))
        {
            RaycastHit Hit;

            if (Physics.Raycast(transform.position, Direction, out Hit, Collider.radius, LineOfSightLayers))
            {
                if (Hit.transform.GetComponent<PlayerMovement>() != null)
                {
                    onGainSight?.Invoke();
                    return true;
                }
            }
        }

        return false;
    }

    private IEnumerator CheckForLineOfSight(PlayerMovement player)
    {
        WaitForSeconds Wait = new WaitForSeconds(0.1f);

        while (!CheckLineOfSight(player))
        {
            yield return Wait;
        }
    }
}