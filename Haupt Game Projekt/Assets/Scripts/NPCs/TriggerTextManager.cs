using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TriggerTextManager : MonoBehaviour
{
    public GameObject triggerText;
    public GameObject DialogueBox;
    public GameObject QuestTitleParent;
    public TextMeshProUGUI QuestTitle;
    public GameObject QuestImage;
    public Image QuestImageObject;

    public DialogueBoxManager boxManager;
    
    void Start()
    {
        boxManager = FindObjectOfType<DialogueBoxManager>();
    }

    
    public void SetTriggerText(bool enable)
    {
        triggerText.SetActive(enable);
    }

    public void SetDialogueBox(bool enable)
    {
        DialogueBox.SetActive(enable);
        boxManager = FindObjectOfType<DialogueBoxManager>();
    }

    public void PlayDialogue(QuestOBJ questObj)
    {
        boxManager.playDialogue(questObj);
    }

    public void SetQuestTitle(string title)
    {
        //boxManager.SetQuestTitle(title);
        QuestTitleParent.SetActive(true);
        QuestTitle.text = title;
    }

    public void SetQuestImage(Sprite image)
    {
        //boxManager.SetQuestImage(image);
        QuestImage.SetActive(true);
        QuestImageObject.sprite = image;
    }

    public void FinishQuest()
    {
        //boxManager.SetFinishedQuest();
        QuestImage.SetActive(false);
        QuestTitleParent.SetActive(false);
    }
}
