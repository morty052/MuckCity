using UnityEngine;

public class Generator : Equipment
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Interact()
    {
        if (!_canInteract) return;
        Debug.Log("Interacting with " + gameObject.name);
        _actionText.SetText("Turn Off");

        if (TryGetComponent(out QuestItem questItem))
        {
            OnInteracted?.Invoke(questItem._questItemData._name);
        }
    }

    void OnTriggerExit(Collider other)
    {
        _actionText.HideInteractionPrompt();
    }


    public bool PlayerInRange() => Vector3.Distance(transform.position, Player.Instance.transform.position) < 0.5f;

}
