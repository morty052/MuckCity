using UnityEngine;
using UnityEngine.Events;

public class ShopTrigger : MonoBehaviour, IInteractable
{

    public IInteractable _parent;

    public bool CanInteract => true;

    public string InteractionPrompt => throw new System.NotImplementedException();

    public GameObject GameObject => gameObject;

    public bool IsHighlighted { get; }

    bool _isQuestItem;

    public bool IsQuestItem { get; set; }

    void Awake()
    {
        _parent = GetComponentInParent<Shop>();
    }

    public void HideInteractionPrompt()
    {
        _parent.HideInteractionPrompt();
    }

    public void Interact()
    {
        _parent.Interact();
    }

    public void PrepareInteraction()
    {
        _parent.PrepareInteraction();
    }

    public void ToggleDrawAttention()
    {
        throw new System.NotImplementedException();
    }
}
