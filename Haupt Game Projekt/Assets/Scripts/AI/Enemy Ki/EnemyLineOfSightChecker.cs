using System;
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
    private const String Player = "Player";
    public FireChecker fireChecker;
    private bool isactive;


    private void Awake()
    {
        fireChecker.onCoverChange += setbool;
        Collider = GetComponent<SphereCollider>();
    }


    void setbool(bool var)
    {
        isactive = !var;
    }

    private void OnTriggerEnter(Collider other)
    {
       if (isactive)
        {

            if (other.gameObject.CompareTag(Player))
            {
            Debug.Log("qqqqq");
                if (!CheckLineOfSight(other.transform))
                {
                    CheckForLineOfSightCoroutine = StartCoroutine(CheckForLineOfSight(other.transform));
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isactive)
        {

            if (other.gameObject.CompareTag(Player))
            {
                OnLoseSight?.Invoke();
                if (CheckForLineOfSightCoroutine != null)
                {
                    StopCoroutine(CheckForLineOfSightCoroutine);
                }
            }
        }
    }

    private bool CheckLineOfSight(Transform player)
    {
      
                    onGainSight?.Invoke();
                    Debug.Log("isdone");
                    return true;
      

      
    }

    private IEnumerator CheckForLineOfSight(Transform Player)
    {
        WaitForSeconds Wait = new WaitForSeconds(0.1f);

        while (!CheckLineOfSight(Player))
        {
            yield return Wait;
        }
    }
}