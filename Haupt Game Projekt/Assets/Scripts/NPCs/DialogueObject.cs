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

 
    public void OnEnable()
    {
        switch (data.DialogueNumber) {
            case 1:
                playDialogue(Dialogue1);
                currDialogue = Dialogue1;
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

