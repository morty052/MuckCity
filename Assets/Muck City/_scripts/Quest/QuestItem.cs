using UnityEngine;

[System.Serializable]
public struct QuestItemData
{

    public string _name;


    public QuestItemData(string name)
    {
        _name = name;
    }
}
public class QuestItem : MonoBehaviour
{
    public QuestStep _tiedQuestStep;
    public QuestItemData _questItemData;
}
