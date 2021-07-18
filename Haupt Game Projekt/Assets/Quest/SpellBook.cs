using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellBook : MonoBehaviour
{
 /*void Update()
    {
        transform.Rotate(0, 50*Time.deltaTime, 0);
    }*/

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            gameObject.SetActive(false);
        }
        
    }   
}
