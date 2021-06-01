using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawning : MonoBehaviour
{
    public float seconds = 2f;
    public GameObject player;
    void Start(){
        
        StartCoroutine(SpawnWait());
    }
    
    IEnumerator SpawnWait()
    {
        player.SetActive(false);
        yield return new WaitForSeconds(seconds);
        player.SetActive(true);
    }
}
