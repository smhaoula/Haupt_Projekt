using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC1 : MonoBehaviour
{
    public GameObject triggerText;
    public GameObject DialogueObject;
    private void OnTriggerStay(Collider other)
    {
     
        if (other.gameObject.tag == "Player")
        {
            triggerText.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E)) {
                other.gameObject.GetComponent<PlayerData>().DialogueNumber = 1;
                DialogueObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        triggerText.SetActive(false);
    }
}
