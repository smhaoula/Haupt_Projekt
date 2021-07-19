using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Quest
{
    public class StoryManager : MonoBehaviour
{

    public QuestInfo[] QuestTemplate;

    public QuestInfo GetRandomQuest()
    {
        int length = QuestTemplate.Length-1;
        
        int select = Random.Range(0,length);
        return QuestTemplate[Random.Range(0, QuestTemplate.Length)];

    }
}
[System.Serializable]
    public class QuestInfo
    {
        public GameObject questGameObject;
        public Sprite QuestObjectPicture;
        public string questTitle;
        public string NPCName;
        public string questTag;
        public QuestOBJ startDialogue;
        public QuestOBJ middleDialogue;
        public QuestOBJ endDialogue;
        public QuestOBJ thanksDialogue;

    }

}
