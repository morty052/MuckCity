using UnityEngine;

public interface IEquipment
{
    bool CanInteract { get; }

    public GameObject GameObject { get; }
    string InteractionPrompt { get; }
    public bool IsHighlighted { get; }

    public void ToggleDrawAttention();
    public void PrepareInteraction();

    public void Interact();

    public void HideInteractionPrompt();

}
