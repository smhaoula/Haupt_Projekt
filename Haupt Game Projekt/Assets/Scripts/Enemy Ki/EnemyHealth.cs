using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyHealth : MonoBehaviour
{
    public float enemyHealth = 100f;
    Enemyki enemyAi;

    private void Start()
    {
        enemyAi = GetComponent<Enemyki>();
    }
    public void DeducHealth(float deducHealth)
    {
        enemyHealth -= deducHealth;
        if (enemyHealth <= 0)
        {
            EnemyDead();
        }
    }

   
    void EnemyDead()
    {
        enemyAi.EnemyDeathanim();
        Destroy(gameObject);
        
    }
}
