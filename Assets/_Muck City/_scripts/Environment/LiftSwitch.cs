using UnityEngine;

public class LiftSwitch : MonoBehaviour, IInteractable
{
    [SerializeField] private Lift _lift;
    [SerializeField] private bool _canInteract = true;
    string _interactionPrompt => "Use Lift";
    public bool CanInteract => _canInteract;

    bool isHighlighted = false;
    public bool IsHighlighted { get => isHighlighted; set => isHighlighted = value; }

    public string InteractionPrompt => _interactionPrompt;

    public GameObject GameObject => gameObject;

    bool _isQuestItem;

    public bool IsQuestItem { get; set; }

    public void HideInteractionPrompt()
    {
        HudManager.Instance.HideInteractPrompt();
    }

    public void Interact()
    {
        HideInteractionPrompt();
        Player.Instance.transform.parent = _lift.transform;
        Player.Instance.ToggleInputLock();
        StartLift();
        // Player.Instance.MoveToPosition(_lift._centerPoint.transform.position, false, () => StartLift());
    }

    void StartLift()
    {
        _lift.Move(() => Player.Instance.ToggleInputLock());
    }

    public void PrepareInteraction()
    {
        HudManager.Instance.ShowInteractPrompt(_interactionPrompt);
    }

    public void ToggleDrawAttention()
    {
        Debug.Log("Toggle Draw Attention");
        isHighlighted = !isHighlighted;
    }
}
