using UnityEngine;

public class Quest
{
    public QuestInfoSo questInfoSo;

    public QuestState questState;

    public int currentQuestStepIndex = 0;

    public Quest(QuestInfoSo questInfoSo)
    {
        this.questInfoSo = questInfoSo;
        this.currentQuestStepIndex = 0;
        this.questState = QuestState.REQUIREMENTS_NOT_MET;
    }

    public void MoveToNextStep()
    {
        currentQuestStepIndex++;
    }

    public bool CurrentStepExists()
    {
        return currentQuestStepIndex < questInfoSo.questStepPrefabs.Length;
    }

    public void InstantiateCurrentQuestStep(Transform parentTransform)
    {
        GameObject questStepPrefab = GetCurrentQuestStepPrefab();
        if (questStepPrefab != null)
        {
            QuestStep questStep = Object.Instantiate(questStepPrefab, parentTransform)
             .GetComponent<QuestStep>();
            questStep.InitializeQuest(questInfoSo._id);
        }
    }

    private GameObject GetCurrentQuestStepPrefab()
    {
        GameObject questStepPrefab = null;
        if (CurrentStepExists())
        {
            questStepPrefab = questInfoSo.questStepPrefabs[currentQuestStepIndex];
        }
        else
        {
            Debug.LogError("No more quest steps to instantiate!");
        }
        return questStepPrefab;
    }
}
