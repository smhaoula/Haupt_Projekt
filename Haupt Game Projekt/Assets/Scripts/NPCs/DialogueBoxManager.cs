using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;



public class DialogueBoxManager : MonoBehaviour
{

    private QuestOBJ currDialogue=null;
    private int currentDialogueNum = 0;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI DialogueText;

    public void DisableBox()
    {
        gameObject.SetActive(false);
    }


    public void playDialogue(QuestOBJ dialogueOBJ)
    {
        currDialogue = dialogueOBJ;
        nameText.text = dialogueOBJ.CharacterName;
        if (currentDialogueNum < dialogueOBJ.Dialogues.Length)
        {
            DialogueText.text = dialogueOBJ.Dialogues[currentDialogueNum];
        }
        else
        {
            currentDialogueNum = 0;
            currDialogue.npc.GetComponent<NPCQuestManager>().isInDialogue= false;
            
            //Debug.Log(currDialogue.npc.GetComponent<NPCQuestManager>().isInDialogue);
            //Debug.Log(currDialogue.npc.GetComponent<NPCQuestManager>().questPhase);
            
            currDialogue.npc.GetComponent<NPCQuestManager>().ChangeQuestPhase();
            
            //Debug.Log(currDialogue.npc.GetComponent<NPCQuestManager>().questPhase);


            currDialogue = null;
            this.gameObject.SetActive(false);
        }

    }

    public void next()
    {
        if (currDialogue != null)
        {
            currentDialogueNum++;
            playDialogue(currDialogue);
        }
    }
}

