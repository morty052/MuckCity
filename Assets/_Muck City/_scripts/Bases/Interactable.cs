using System;
using UnityEngine;

public class Interactable : MonoBehaviour, IInteractable, IFindable
{

    [SerializeField] protected bool _canInteract = true;
    [SerializeField] protected string _interactionPrompt;
    public bool CanInteract => _canInteract;

    public GameObject GameObject => gameObject;

    public string InteractionPrompt => _interactionPrompt;

    public bool IsHighlighted => _actionText.IsHighlighted;

    public ActionText _actionText;

    public Action<string> OnInteracted;

    [SerializeField] bool _isQuestItem;

    public bool IsQuestItem { get => _isQuestItem; set => _isQuestItem = value; }



    void Start()
    {
        _actionText.SetText(_interactionPrompt);
    }

    public virtual void HideInteractionPrompt()
    {
        _actionText.HideInteractionPrompt();
    }

    public virtual void Interact()
    {
        Debug.Log("Interacted");
    }

    public virtual void PrepareInteraction()
    {
        _actionText.ShowInteractionPrompt();
        Player.Instance.SetInteractableObject(this);
    }

    public virtual void ToggleDrawAttention()
    {
        _actionText.ToggleWhiteDot();
    }

    public void SetupInteractionListener(Action<string> action)
    {
        OnInteracted += action;
    }

    public void RemoveInteractionListener(Action<string> action)
    {
        OnInteracted -= action;
    }
}
