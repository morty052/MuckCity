using System;
using UnityEngine;

public struct QuestItemTag
{
    public string _tag;
    public QuestItemTag(string tag)
    {
        _tag = tag;
    }
}
public class QuestItem : MonoBehaviour
{
    public QuestStep _tiedQuestStep;
    public QuestItemTag _questItemData;

    public Action OnInteract;


}
