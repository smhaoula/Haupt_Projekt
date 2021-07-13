using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTextManager : MonoBehaviour
{
    public GameObject triggerText;
    public GameObject DialogueBox;

    
    public void SetTriggerText(bool enable)
    {
        triggerText.SetActive(enable);
    }

    public void SetDialogueBox(bool enable)
    {
        DialogueBox.SetActive(enable);
    }
}
