using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class DogCaller : MonoBehaviour, IInteractable
{

    [SerializeField] ActionText _actionText;
    [SerializeField] bool _canInteract;

    public bool CanInteract => _canInteract;

    public GameObject GameObject => gameObject;

    public string InteractionPrompt => "Call Rover";

    public bool IsHighlighted => _actionText.IsHighlighted;

    public Action<string> OnInteracted;

    [SerializeField] bool _isQuestItem;

    public bool IsQuestItem { get => _isQuestItem; set => _isQuestItem = value; }

    [SerializeField] bool _dogIsOnLeash = true;

    void OnTriggerEnter(Collider other)
    {
        PrepareInteraction();
    }

    void OnTriggerExit(Collider other)
    {
        HideInteractionPrompt();
    }

    [Button]
    void CallDog()
    {
        Dog.Instance.ToggleAccessToPlayer();
        _dogIsOnLeash = false;
    }

    public void ToggleDrawAttention()
    {
        _actionText.ToggleWhiteDot();
    }

    public void PrepareInteraction()
    {
        if (!_dogIsOnLeash) return;
        _actionText.ShowInteractionPrompt();
        Player.Instance.SetInteractableObject(this);
    }

    public void Interact()
    {
        HideInteractionPrompt();
        CallDog();
    }

    public void HideInteractionPrompt()
    {
        _actionText.HideInteractionPrompt();
        Player.Instance.SetInteractableObject(null);
    }
}
