using UnityEngine;

public class DistrictNoticeBoard : MonoBehaviour, IInteractable
{
    [SerializeField] District _district;

    [SerializeField] bool _canInteract = true;

    public bool CanInteract => _canInteract;

    public string InteractionPrompt => "Place Marker";

    public void HideInteractionPrompt()
    {
        HudManager.Instance.HideInteractPrompt();
    }


    void OnTriggerExit(Collider other)
    {
        HudManager.Instance.HideInteractPrompt();
    }


    public void Interact()
    {
        if (!CanInteract) return;
        HideInteractionPrompt();
        _district.OnDeliveryMarkerPlaced();
        _canInteract = false;
    }

    public void PrepareInteraction()
    {
        if (!CanInteract) return;
        HudManager.Instance.ShowInteractPrompt(InteractionPrompt);
    }
}
