using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC1 : MonoBehaviour
{
    public GameObject triggerText;
    public GameObject DialogueObject;
    public bool hasTalked = false;
    public bool isInDialogue = false;
    private void OnTriggerStay(Collider other)
    {
     
        if (other.gameObject.tag == "Player" && !isInDialogue)
        {
            triggerText.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E)) {
                isInDialogue = true;

                if(!hasTalked)
                {
                    other.gameObject.GetComponent<PlayerData>().DialogueNumber = 1;
                    DialogueObject.SetActive(true);
                    triggerText.SetActive(false);
                }
                else{
                    other.gameObject.GetComponent<PlayerData>().DialogueNumber = 1.5f;
                    DialogueObject.SetActive(true);
                    triggerText.SetActive(false);
                }
                
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        triggerText.SetActive(false);
    }
}
