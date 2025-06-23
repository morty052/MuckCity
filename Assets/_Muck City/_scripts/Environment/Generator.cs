using UnityEngine;

public class Generator : Equipment
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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


    public bool PlayerInRange() => Vector3.Distance(transform.position, Player.Instance.transform.position) < 0.5f;

}
