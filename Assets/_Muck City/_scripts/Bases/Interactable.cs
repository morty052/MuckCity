

using System;
using UnityEngine;

public interface IInteractable
{

    bool CanInteract { get; }

    public GameObject GameObject { get; }
    string InteractionPrompt { get; }
    public bool IsHighlighted { get; }
    public bool IsQuestItem { get; set; }


    public void ToggleDrawAttention();
    public void PrepareInteraction();

    public void Interact();

    public void HideInteractionPrompt();


}
