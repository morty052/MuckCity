using Sirenix.OdinInspector;
using UnityEngine;

public class Generator : Equipment
{
    [TabGroup("Lights")]
    [SerializeField] GameObject[] lights;
    [SerializeField] bool _debug;
    public override void Interact()
    {
        if (!_canInteract) return;
        _actionText.SetText("Turn Off");

        if (IsQuestItem)
        {
            QuestItem questItem = GetComponent<QuestItem>();
            OnInteracted?.Invoke(questItem._questItemData._tag);
        }
    }

    void OnTriggerExit(Collider other)
    {
        _actionText.HideInteractionPrompt();
    }


}
