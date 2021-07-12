using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestOBJ
{
    public string[] Dialogues;
    public string CharacterName;
}

public class NPCQuestManager : MonoBehaviour
{
    public GameObject triggerText;
    public GameObject DialogueObject;
    public GameObject questGameObject;
    public Sprite QuestObjectPicture;
    public bool hasTalked = false;
    public bool isInDialogue = false;
    public string NPCName;
    public QuestOBJ startDialogue;
    public QuestOBJ middleDialogue;
    public QuestOBJ endDialogue;
    public QuestOBJ thanksDialogue;
    [Range(0,3)]
    public int questPhase;

    void Start()
    {

    }
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
