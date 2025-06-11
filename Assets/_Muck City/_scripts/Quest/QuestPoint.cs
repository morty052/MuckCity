using System;
using UnityEngine;

public class QuestPoint : MonoBehaviour
{
    [SerializeField] private QuestInfoSo _questInfoForPoint;
    public QuestStep _tiedQuestStep;
    public QuestItemData _questItemData;

    private string _questId;

    private QuestState _currentQuestState;

    [SerializeField] private bool _startPoint;

    [SerializeField] private bool _endPoint;

    private void Awake()
    {
        if (_questInfoForPoint != null)
        {
            _questId = _questInfoForPoint._id;
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_startPoint)
            {
                GameEventsManager.Instance._questEvents.StartQuest(_questId);
            }

            else
            {
                _tiedQuestStep.OnQuestItemInteracted(_questItemData._name);
            }
            Destroy(gameObject);
        }
    }

}
