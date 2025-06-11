using UnityEngine;

public class InteractionTrigger : MonoBehaviour
{
    IInteractable _parent;

    void Awake()
    {
        _parent = GetComponentInParent<IInteractable>();
    }

    public void Interact()
    {
        _parent.Interact();
    }

    public void PrepareInteraction()
    {
        _parent.PrepareInteraction();
    }

    public void HideInteractionPrompt()
    {
        _parent.HideInteractionPrompt();
    }
}
