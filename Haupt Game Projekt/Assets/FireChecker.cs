using System;
using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class FireChecker : MonoBehaviour
{
    private SphereCollider fireCollider;
   [SerializeField] private float FieldOfView = 360f;
   [SerializeField] private LayerMask LineOfSightLayers;

    private const String Fire = "Fire";
   
    public SignEvent OnGainSight;
    public delegate void SignEvent();
    public SignEvent OnLoseSight;

    private Coroutine CheckForLineOfSightCoroutine;
    public Action<bool> onCoverChange;

    private void Awake()
    {
        fireCollider = GetComponent<SphereCollider>();  
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(Fire))
        {
            if (!CheckLineOfSight(other.transform))
            {
                Debug.Log(" i can see Fire");   
                onCoverChange?.Invoke(true);
                CheckForLineOfSightCoroutine = StartCoroutine(CheckForLineOfSight(other.transform));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag( Fire))
        {
            OnLoseSight?.Invoke();
            if (CheckForLineOfSightCoroutine != null)
            {  
                onCoverChange?.Invoke(false);
                StopCoroutine(CheckForLineOfSightCoroutine);
            }
        }
    }

    private bool CheckLineOfSight(Transform fire)
    {
        Vector3 myPosition = transform.position + Vector3.up * 1;
        Vector3 Direction = (fire.position - myPosition).normalized;

        if (Vector3.Angle(transform.forward, Direction) < FieldOfView / 2)
        {
            if (Physics.Raycast(myPosition, Direction, out RaycastHit Hit, fireCollider.radius, LineOfSightLayers))
            {
                if (Hit.transform.CompareTag(Fire))
                {
                    OnGainSight?.Invoke();
                    return true;
                }
            }
        }

        return false;
    }

    private IEnumerator CheckForLineOfSight(Transform fire)
    {
        WaitForSeconds Wait = new WaitForSeconds(0.1f);

        while (!CheckLineOfSight(fire))
        {
            yield return Wait;
        }
    }
}