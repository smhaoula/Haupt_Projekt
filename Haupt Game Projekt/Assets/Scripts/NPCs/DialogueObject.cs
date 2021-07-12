using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

[Serializable]
public class DialogueOBJ
{
    public string[] Dialogues;
    public string CharacterName;
    public int questNumber;
}

public class DialogueObject : MonoBehaviour
{

    public PlayerData data;

    private DialogueOBJ currDialogue=null;
    private int currentDialogueNum;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI DialogueText;

    [Header("Dialogue objects")]

    public DialogueOBJ Dialogue1;
    public DialogueOBJ Dialogue1o5;

    [Header("NPCs")]
    public NPC1 nPC1;

    public void OnEnable()
    {
        switch (data.DialogueNumber) {
            case 1:
                playDialogue(Dialogue1);
                currDialogue = Dialogue1;
                break;
            case 1.5f:
                playDialogue(Dialogue1o5);
                currDialogue = Dialogue1o5;
                break;
        }
    }

    void playDialogue(DialogueOBJ dialogueOBJ)
    {
        nameText.text = dialogueOBJ.CharacterName;
        if (currentDialogueNum < dialogueOBJ.Dialogues.Length)
        {
            DialogueText.text = dialogueOBJ.Dialogues[currentDialogueNum];
        }
        else
        {
            switch(data.DialogueNumber)
            {
                case 1:
                    nPC1.hasTalked=true;
                    nPC1.isInDialogue=false;
                    break;
                case 1.5f:
                    nPC1.isInDialogue=false;
                    break;
            }
            currentDialogueNum = 0;
            data.DialogueNumber = 0;
            data.questNumber = currDialogue.questNumber;
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

