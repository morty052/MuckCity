using UnityEngine;
using UnityEngine.Events;

public class ShopTrigger : MonoBehaviour, IInteractable
{

    public IInteractable _parent;

    public bool CanInteract => true;

    public string InteractionPrompt => throw new System.NotImplementedException();


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

}
