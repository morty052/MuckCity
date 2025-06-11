using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private Dictionary<string, Quest> _questMap = new();


    private void Awake()
    {
        _questMap = CreateQuestMap();
    }

    private void OnEnable()
    {
        GameEventsManager.Instance._questEvents.OnQuestStarted += StartQuest;
        GameEventsManager.Instance._questEvents.OnAdvanceQuest += AdvanceQuest;
        GameEventsManager.Instance._questEvents.OnQuestFinished += FinishQuest;
        GameEventsManager.Instance._questEvents.OnQuestStateChange += QuestStateChanged;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance._questEvents.OnQuestStarted -= StartQuest;
        GameEventsManager.Instance._questEvents.OnAdvanceQuest -= AdvanceQuest;
        GameEventsManager.Instance._questEvents.OnQuestFinished -= FinishQuest;
        GameEventsManager.Instance._questEvents.OnQuestStateChange -= QuestStateChanged;
    }

    private void StartQuest(string id)
    {
        Quest quest = GetQuestById(id);
        quest.InstantiateCurrentQuestStep(this.transform);
        ChangeQuestState(quest.questInfoSo._id, QuestState.IN_PROGRESS);
    }

    private void ChangeQuestState(string id, QuestState state)
    {
        Quest quest = GetQuestById(id);
        quest.questState = state;
        GameEventsManager.Instance._questEvents.QuestStateChanged(quest);
    }

    private void AdvanceQuest(string id)
    {
        Debug.Log("Advanced Quest: " + id);

        Quest quest = GetQuestById(id);

        quest.MoveToNextStep();

        if (quest.CurrentStepExists())
        {
            quest.InstantiateCurrentQuestStep(this.transform);
        }
        else
        {
            ChangeQuestState(quest.questInfoSo._id, QuestState.CAN_FINISH);
        }
    }

    private void FinishQuest(string id)
    {
        OnScreenDebugger.Instance.Log("Finished Quest: " + id);
    }

    private void QuestStateChanged(Quest quest)
    {
        OnScreenDebugger.Instance.Log("Quest State Changed: " + quest.questInfoSo._id);
    }

    private Dictionary<string, Quest> CreateQuestMap()
    {
        QuestInfoSo[] allQuests = Resources.LoadAll<QuestInfoSo>("Quests");
        Dictionary<string, Quest> idQuestMap = new();
        foreach (QuestInfoSo questInfoSo in allQuests)
        {
            if (idQuestMap.ContainsKey(questInfoSo._id))
            {
                Debug.LogWarning("Duplicate id: " + questInfoSo._id);
            }

            idQuestMap.Add(questInfoSo._id, new Quest(questInfoSo));
        }

        return idQuestMap;

    }


    private Quest GetQuestById(string id)
    {
        Quest quest = _questMap[id];
        if (quest == null)
        {
            Debug.LogError("No quest with id: " + id);
        }

        return quest;
    }
}
