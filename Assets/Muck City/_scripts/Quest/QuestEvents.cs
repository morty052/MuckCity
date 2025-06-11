using System;
using UnityEngine;

public class QuestEvents
{
    public event Action<string> OnQuestStarted;
    public void StartQuest(string id)
    {
        OnQuestStarted?.Invoke(id);
    }
    public event Action<string> OnAdvanceQuest;
    public void AdvanceQuest(string id)
    {
        OnAdvanceQuest?.Invoke(id);
    }
    public event Action<string> OnQuestFinished;
    public void FinishQuest(string id)
    {
        OnQuestFinished?.Invoke(id);
    }
    public event Action<Quest> OnQuestStateChange;
    public void QuestStateChanged(Quest quest)
    {
        OnQuestStateChange?.Invoke(quest);
    }

}
