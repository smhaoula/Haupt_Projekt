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
    ObjectPooler objectPooler;
    
    public TriggerTextManager textManager;
    public GameObject questGameObject;
    public string questTag;
    public Sprite QuestObjectPicture;
    public string questTitle;
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
    public PlayerMovement player;
    public bool changeDialogue;
    

    

    void Start()
    {
        QuestInfo questInfo = FindObjectOfType<StoryManager>().GetRandomQuest();
        objectPooler = ObjectPooler.Instance;

        NPCName = questInfo.NPCName;
        questTag = questInfo.questTag;
        questGameObject = questInfo.questGameObject;
        QuestObjectPicture = questInfo.QuestObjectPicture;
        questTitle=questInfo.questTitle;
        
        startDialogue = questInfo.startDialogue;
        startDialogue.CharacterName = NPCName;
        startDialogue.npc = this.gameObject;

        middleDialogue = questInfo.middleDialogue;
        middleDialogue.CharacterName = NPCName;
        middleDialogue.npc = gameObject;

        endDialogue = questInfo.endDialogue;
        endDialogue.CharacterName = NPCName;
        endDialogue.npc = gameObject;

        thanksDialogue = questInfo.thanksDialogue;
        thanksDialogue.CharacterName = NPCName;
        thanksDialogue.npc = gameObject;

        textManager = FindObjectOfType<TriggerTextManager>();
        questPhase = 0;
        finished = false;
    }
    private void OnTriggerStay(Collider other)
    {
     
        if (other.gameObject.CompareTag("Player") && !isInDialogue)
        {
            other.GetComponent<PlayerMovement>().talking = true;
            if(other.gameObject.GetComponent<PlayerMovement>().pickedUpQuestObject)
                {
                    questPhase = 2;
                }
            textManager.SetTriggerText(true);
            if (Input.GetKeyDown(KeyCode.E)) {
                isInDialogue = true;
                
                textManager.SetDialogueBox(true);
                textManager.SetTriggerText(true);
                

                switch(questPhase)
                {
                    case 0:
                        currentQuest = startDialogue;
                        textManager.PlayDialogue(currentQuest);
                        textManager.SetQuestTitle(questTitle);
                        SpawnQuestObject();
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

                
                changeDialogue = true;
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
        Debug.Log("ChangeQuest aufgerufen");
    }

    private void OnTriggerExit(Collider other)
    {
        textManager.SetTriggerText(false);
        player = FindObjectOfType<PlayerMovement>();
        player.talking = false;
        if(changeDialogue)
        {
            isInDialogue = false;
            ChangeQuestPhase();
            changeDialogue = false;
        }
        
    }

    public void SpawnQuestObject()
    {
        Vector3 spawnPos = (UnityEngine.Random.insideUnitSphere * 5 + transform.position);
        RaycastHit hit = new RaycastHit();
        var p = new Vector3(spawnPos.x, 100, spawnPos.z);
        Ray ray = new Ray(p, Vector3.down*200);
        int layer_mask = LayerMask.GetMask("Terrain");
        if(Physics.Raycast(ray, out hit, Mathf.Infinity)){
            spawnPos.y = hit.point.y + 1.5f;
            GameObject questObject = objectPooler.SpawnFromPool(questTag, spawnPos, Quaternion.Euler(0,0,0));
    }
}
}
