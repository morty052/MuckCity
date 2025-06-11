using GogoGaga.OptimizedRopesAndCables;
using UnityEngine;

public class RoverInterface : MonoBehaviour, IInteractable
{
    public Rope _cable;
    [SerializeField] QuestItem _questItem;

    public bool CanInteract => true;

    public string InteractionPrompt => "Interact with Rover";

    public void DisconnectCable()
    {
        _cable.SetEndPoint(null);
        Debug.Log("Cable disconnected");
    }

    public void HideInteractionPrompt()
    {
        throw new System.NotImplementedException();
    }

    public void Interact()
    {
        if (_questItem != null)
        {
            _questItem._tiedQuestStep.OnQuestItemInteracted(_questItem._questItemData._name);
        }
    }

    public void PrepareInteraction()
    {
        HudManager.Instance.ShowInteractPrompt(InteractionPrompt);
    }
}
