using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
[Serializable]
public class QuestOBJ
{
    public string[] Dialogues;
    public string CharacterName;
    public GameObject npc;
}

public class NPCQuestManager : MonoBehaviour
{
    
    public GameObject triggerText;
    public GameObject DialogueBox;
    public GameObject questGameObject;
    public Sprite QuestObjectPicture;
    public string questTitle;
    public bool hasTalked = false;
    public bool isInDialogue = false;
    public string NPCName;
    public QuestOBJ startDialogue;
    public QuestOBJ middleDialogue;
    public QuestOBJ endDialogue;
    public QuestOBJ thanksDialogue;
    public QuestOBJ currentQuest;
    [Range(0,3)]
    public int questPhase;
    public bool finished;

    

    void Start()
    {
        triggerText = GameObject.Find("triggerText").GetComponent<GameObject>();
        DialogueBox = GameObject.Find("DialogueBox").GetComponent<GameObject>();
        questPhase = 0;
        currentQuest = startDialogue;
    }
    private void OnTriggerStay(Collider other)
    {
     
        if (other.gameObject.tag == "Player" && !isInDialogue)
        {
            /*if(other.gameObject.hasBook)
                {
                    questPhase = 2;
                }*/
            triggerText.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E)) {
                isInDialogue = true;
                DialogueBox.SetActive(true);
                triggerText.SetActive(false);
                

                switch(questPhase)
                {
                    case 0:
                        currentQuest = startDialogue;
                        DialogueBox.GetComponent<DialogueBoxManager>().playDialogue(currentQuest);
                        DialogueBox.GetComponent<DialogueBoxManager>().SetQuestTitle(questTitle);
                        DialogueBox.GetComponent<DialogueBoxManager>().SetQuestImage(QuestObjectPicture);
                        break;
                    case 1:
                        currentQuest = middleDialogue;
                        DialogueBox.GetComponent<DialogueBoxManager>().playDialogue(currentQuest);
                        break;
                    case 2:
                        currentQuest = endDialogue;
                        DialogueBox.GetComponent<DialogueBoxManager>().playDialogue(currentQuest);
                        finished = true;
                        DialogueBox.GetComponent<DialogueBoxManager>().SetFinishedQuest();
                        ScoreScript.scoreValue += 20;
                        break;
                    case 3: 
                        currentQuest = thanksDialogue;
                        DialogueBox.GetComponent<DialogueBoxManager>().playDialogue(currentQuest);
                        break;
                }

                
                
            }
        }
    }

    public void ChangeQuestPhase()
    {
        
        if(questPhase == 0)
        {
            questPhase++;
        }
        if(finished)
        {
            questPhase = 3;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        triggerText.SetActive(false);
    }
}
