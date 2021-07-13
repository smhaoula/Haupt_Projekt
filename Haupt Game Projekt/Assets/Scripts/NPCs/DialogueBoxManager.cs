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
    public GameObject QuestTitleParent;
    public TextMeshProUGUI QuestTitle;
    public GameObject QuestImage;
    public Image QuestImageObject;

    public void DisableBox()
    {
        gameObject.SetActive(false);
    }

    public void SetQuestImage(Sprite questImage)
    {
        QuestImage.SetActive(true);
        QuestImageObject.sprite = questImage;
        
    }

    public void SetQuestTitle(string questTitle)
    {
        QuestTitleParent.SetActive(true);
        QuestTitle.text = questTitle;
    }

    public void SetFinishedQuest()
    {
        QuestImage.SetActive(false);
        QuestTitleParent.SetActive(false);
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
            currDialogue.npc.GetComponent<NPCQuestManager>().ChangeQuestPhase();


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

