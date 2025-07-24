using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class Interactable : MonoBehaviour, IInteractable, IFindable
{


    [TabGroup("Interaction")]
    [SerializeField] protected bool _canInteract = true;
    [TabGroup("Interaction")]
    [SerializeField] protected string _interactionPrompt;
    [TabGroup("Interaction")]
    [SerializeField] Pos _promptPos;
    [TabGroup("Interaction")]
    public bool CanInteract => _canInteract;
    [TabGroup("Interaction")]

    public GameObject GameObject => gameObject;
    [TabGroup("Interaction")]

    public string InteractionPrompt => _interactionPrompt;
    [TabGroup("Interaction")]

    public bool IsHighlighted => _actionText.IsHighlighted;
    [TabGroup("Interaction")]

    public ActionText _actionText;

    public Action<string> OnInteracted;
    [TabGroup("Interaction")]
    [SerializeField] bool _isQuestItem;

    public bool IsQuestItem { get => _isQuestItem; set => _isQuestItem = value; }



    public virtual void Start()
    {
        Invoke(nameof(SetupActionText), 1f);
    }

    void SetupActionText()
    {
        _actionText = HudManager.Instance._actionText;
        // if (_actionText != null)
        // {
        //     _actionText.SetText(_interactionPrompt);
        // }
    }

    public virtual void HideInteractionPrompt()
    {
        _actionText.gameObject.SetActive(false);
    }

    public virtual void Interact()
    {
        Debug.Log("Interacted");
    }

    public virtual void PrepareInteraction()
    {
        if (!_canInteract) return;
        ShowInteractPrompt();
        Player.Instance.SetInteractableObject(this);
    }

    public void ShowInteractPrompt()
    {
        _actionText.transform.position = transform.TransformPoint(_promptPos.position);
        _actionText.SetText(_interactionPrompt ?? "Interact");
        _actionText.gameObject.SetActive(true);
    }

    public virtual void ToggleDrawAttention()
    {
        if (_actionText == null || !_canInteract) return;
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

    public virtual void ToggleCanInteract(bool state)
    {
        _canInteract = state;
    }
}
