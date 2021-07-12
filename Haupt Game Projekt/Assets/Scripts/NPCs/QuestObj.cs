using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

[Serializable]
public class QuestObject
{
    public string questTitle;
    //public string questobj1;

}
public class QuestObj : MonoBehaviour
{
    public GameObject questObj;
    public TextMeshProUGUI QuestTitle;
    public QuestObject[] questObjs;
    public void StartNewQuest(QuestObject tempobj)
    {
        QuestTitle.text = tempobj.questTitle;
        questObj.SetActive(true);
        Invoke("CloseQuest", 7f);
    }

    private void CloseQuest()
    {
        questObj.SetActive(false);
    }
}
