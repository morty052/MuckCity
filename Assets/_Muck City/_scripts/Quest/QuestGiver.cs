using UnityEngine;

public class QuestGiver : MonoBehaviour
{
    [SerializeField] QuestInfoSo _questInfoSo;
    [SerializeField] QuestStep _questStep;

    public bool HasQuest => _questInfoSo != null;

    [SerializeField] bool _activeQuestHasConvo;

    public bool HasConvoForQuest => _activeQuestHasConvo;

    public void UpdateQuestData(QuestInfoSo questInfoSo, QuestStep questStep, bool hasConvo = false)
    {
        _questInfoSo = questInfoSo;
        _questStep = questStep;
        if (hasConvo)
        {
            _activeQuestHasConvo = true;
        }
        // OnScreenDebugger.Instance.Log(name + " updated quest data with id " + _questInfoSo._id + " quest step " + _questStep);
    }
}
