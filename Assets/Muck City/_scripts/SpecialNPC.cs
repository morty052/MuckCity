using System;
using DialogueEditor;
using UnityEngine;

public class SpecialNPC : NpcCharacter
{
    QuestGiver _questGiver;
    public event Action OnInteractedWithQuestGiver;
    public event Action OnFinishedWithQuestGiver;

    public void Test()
    {
        Debug.Log("Hitting");
    }


    // Update is called once per frame
    protected override void SetupData()
    {
        base.SetupData();
        if (IsQuestGiver)
        {
            _questGiver = GetComponent<QuestGiver>();
        }
    }

    public void UpdateQuestData(QuestInfoSo questInfoSo, QuestStep questStep, NPCConversation conversationForQuest = null)
    {
        bool hasConvo = conversationForQuest != null;
        _questGiver.UpdateQuestData(questInfoSo, questStep, hasConvo);
        if (conversationForQuest != null)
        {
            _activeConversation = conversationForQuest;
        }

    }

    public override void Interact()
    {
        if (IsQuestGiver && _questGiver.HasQuest)
        {
            OnScreenDebugger.Instance.Log("Interacted with quest giver");
            if (_questGiver.HasConvoForQuest)
            {
                StartConversation(_activeConversation);
                OnInteractedWithQuestGiver?.Invoke();
            }
        }
        HideInteractionPrompt();
    }
}
