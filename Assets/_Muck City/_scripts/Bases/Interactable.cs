

public interface IInteractable
{

    bool CanInteract { get; }
    string InteractionPrompt { get; }
    // float InteractionDistance { get; }

    public void PrepareInteraction();

    public void Interact();

    public void HideInteractionPrompt();
}
