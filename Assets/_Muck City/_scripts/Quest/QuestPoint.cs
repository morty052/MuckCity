using System;
using UnityEngine;

public class QuestPoint : MonoBehaviour
{
    [SerializeField] private QuestInfoSo _questInfoForPoint;
    public QuestStep _tiedQuestStep;
    public QuestPointData _questItemData;

    [SerializeField] private string _questId;

    public bool _completesObjective;

    public int _objectiveIndex = 0;

    [SerializeField] private bool _startPoint;

    [SerializeField] private bool _endPoint;
    [SerializeField] private bool _useVisibility = false;

    public Action<string, bool> OnEnterQuestPoint;

    private void Awake()
    {
        if (_questInfoForPoint != null)
        {
            _questId = _questInfoForPoint._id;
        }
        if (_useVisibility)
        {
            GetComponent<Collider>().enabled = false;
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartQuest();
            Destroy(gameObject);
        }
    }


    public void StartQuest()
    {
        if (_startPoint)
        {
            GameEventsManager.Instance._questEvents.StartQuest(_questId);
        }

        else
        {
            // _tiedQuestStep.OnQuestItemInteracted(_questItemData._name);
            OnEnterQuestPoint?.Invoke(_questItemData._name, _completesObjective);
        }
    }
    public void StartQuestByVisibility()
    {
        if (_startPoint)
        {
            GameEventsManager.Instance._questEvents.StartQuest(_questId);
        }

        else
        {
            // _tiedQuestStep.OnQuestItemInteracted(_questItemData._name);
            OnEnterQuestPoint?.Invoke(_questItemData._name, _completesObjective);
        }
    }

}
