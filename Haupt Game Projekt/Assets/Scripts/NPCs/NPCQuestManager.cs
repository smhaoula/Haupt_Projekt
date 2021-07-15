using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using Quest;
[Serializable]
public class QuestOBJ
{
    public string[] Dialogues;
    public string CharacterName;
    public GameObject npc;
}

public class NPCQuestManager : MonoBehaviour
{
    public TriggerTextManager textManager;
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
    Canvas canvas;
    

    

    void Start()
    {
        QuestInfo questInfo = FindObjectOfType<StoryManager>().GetRandomQuest();


        //textManager = GameObject.FindWithTag("UI").GetComponent<TriggerTextManager>();
        //NPCName = "Generic NPC Name";
        NPCName = questInfo.NPCName;
        questGameObject = questInfo.questGameObject;
        QuestObjectPicture = questInfo.QuestObjectPicture;
        //questTitle = "Finde das Zauberbuch";
        questTitle=questInfo.questTitle;
        //startDialogue=new QuestOBJ();
        startDialogue = questInfo.startDialogue;
        startDialogue.CharacterName = NPCName;
        //startDialogue.Dialogues = new string[] {"Hallo furchtloser Held!", "Ich habe leider mein Zauberbuch verloren.", "Kannst du es für mich finden?"};
        startDialogue.npc = gameObject;

        //middleDialogue=new QuestOBJ();
        middleDialogue = questInfo.middleDialogue;
        middleDialogue.CharacterName = NPCName;
        //middleDialogue.Dialogues = new string[] {"Schade das du mein Zauberbuch noch nicht gefunden hast."};
        middleDialogue.npc = gameObject;

        //endDialogue=new QuestOBJ();
        endDialogue = questInfo.endDialogue;
        endDialogue.CharacterName = NPCName;
        //endDialogue.Dialogues = new string[] {"Ja das ist es! Du hast es gefunden!", "Vielen Dank für deine Hilfe.", "Nimm das als Dank."};
        endDialogue.npc = gameObject;

        //thanksDialogue=new QuestOBJ();
        thanksDialogue = questInfo.thanksDialogue;
        thanksDialogue.CharacterName = NPCName;
        //thanksDialogue.Dialogues = new string[] {"Vielen Dank für deine Hilfe!"};
        thanksDialogue.npc = gameObject;

        textManager = FindObjectOfType<TriggerTextManager>();
        questPhase = 0;
        currentQuest = startDialogue;
    }
    private void OnTriggerStay(Collider other)
    {
     
        if (other.gameObject.tag == "Player" && !isInDialogue)
        {
            if(other.gameObject.GetComponent<PlayerMovement>().pickedUpQuestObject)
                {
                    questPhase = 2;
                }
            //triggerText.SetActive(true);
            textManager.SetTriggerText(true);
            if (Input.GetKeyDown(KeyCode.E)) {
                isInDialogue = true;
                //DialogueBox.SetActive(true);
                textManager.SetDialogueBox(true);
                //triggerText.SetActive(false);
                textManager.SetTriggerText(true);
                

                switch(questPhase)
                {
                    case 0:
                        currentQuest = startDialogue;
                        textManager.PlayDialogue(currentQuest);
                        textManager.SetQuestTitle(questTitle);
                        SpawnQuestObject();
                        //textManager.SetQuestImage(QuestObjectPicture);
                        other.gameObject.GetComponent<PlayerMovement>().StartQuest(questGameObject, QuestObjectPicture);
                        break;
                    case 1:
                        currentQuest = middleDialogue;
                        textManager.PlayDialogue(currentQuest);                        
                        break;
                    case 2:
                        currentQuest = endDialogue;
                        textManager.PlayDialogue(currentQuest);
                        finished = true;
                        other.gameObject.GetComponent<PlayerMovement>().FinishQuest();
                        textManager.FinishQuest();
                        ScoreScript.scoreValue += 20;
                        ScoreScript.currentScore +=20;
                        break;
                    case 3: 
                        currentQuest = thanksDialogue;
                        textManager.PlayDialogue(currentQuest);
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
        //triggerText.SetActive(false);
        textManager.SetTriggerText(false);
    }

    public void SpawnQuestObject()
    {
        Vector3 spawnPos = (UnityEngine.Random.insideUnitSphere * 5 + transform.position);
        RaycastHit hit = new RaycastHit();
        var p = new Vector3(spawnPos.x, 100, spawnPos.z);
        Ray ray = new Ray(p, Vector3.down*200);
        if(Physics.Raycast(ray, out hit, Mathf.Infinity)){
            spawnPos.y = hit.point.y + 2f;
            GameObject questObject = Instantiate(questGameObject, spawnPos, Quaternion.Euler(0,0,0));
        }
    }
}
