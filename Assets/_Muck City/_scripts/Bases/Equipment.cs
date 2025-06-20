using UnityEngine;

public class Equipment : MonoBehaviour, IInteractable
{

    public GameObject GameObject => gameObject;

    [SerializeField] protected bool _canInteract = true;

    public bool CanInteract { get => _canInteract; protected set => _canInteract = value; }

    public string InteractionPrompt => "Interact";

    public bool IsHighlighted => _actionText.IsHighlighted;

    public ActionText _actionText;


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

    public void PrepareInteraction()
    {
        if (!_canInteract) return;
        _actionText.ToggleInteractionPrompt();
        // Player.Instance.SetInteractableObject(this);
    }

    public void ToggleDrawAttention()
    {
        _actionText.ToggleWhiteDot();
    }

}
