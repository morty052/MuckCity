using System;
using UnityEngine;

public class Equipment : MonoBehaviour, IInteractable
{

    public GameObject GameObject => gameObject;

    [SerializeField] protected bool _canInteract = true;

    public bool CanInteract { get => _canInteract; protected set => _canInteract = value; }

    public string InteractionPrompt => "Interact";

    public bool IsHighlighted => _actionText.IsHighlighted;

    public ActionText _actionText;

    public Action<string> OnInteracted;

    bool _isQuestItem;

    public bool IsQuestItem { get; set; }


    public virtual void Interact()
    {
        Debug.Log("Interacting with " + gameObject.name);
    }

    public virtual void ToggleInteractPrompt()
    {
        _actionText.ShowHideInteractionPrompt();
    }

    public virtual void HideInteractionPrompt()
    {
        _actionText.HideInteractionPrompt();
        Player.Instance.SetInteractableObject(null);
    }

    public virtual void PrepareInteraction()
    {
        if (!_canInteract) return;
        _actionText.ShowInteractionPrompt();
        // Player.Instance.SetInteractableObject(this);
    }

    public virtual void ToggleDrawAttention()
    {
        if (!_canInteract) return;
        _actionText.ToggleWhiteDot();
    }

    public virtual void ToggleCanInteract()
    {
        _canInteract = !_canInteract;
    }

}
