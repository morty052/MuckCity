using DG.Tweening;
using UnityEngine;

public class DoorSwitch : MonoBehaviour, IInteractable
{

    [SerializeField] GameObject _door;
    [SerializeField] bool _canInteract;
    public bool CanInteract => _canInteract;

    public GameObject GameObject => gameObject;

    public string InteractionPrompt => "";

    [SerializeField] ActionText _actionText;

    public bool IsHighlighted { get => _actionText.IsHighlighted; }

    [SerializeField] bool _isOpen = false;

    public void HideInteractionPrompt()
    {
        _actionText.ToggleInteractionPrompt();
        Player.Instance.SetInteractableObject(null);
    }

    public void Interact()
    {
        if (!_isOpen)
        {
            OpenDoor();
        }

        else
        {
            CloseDoor();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        PrepareInteraction();
    }

    void OnTriggerExit(Collider other)
    {
        HideInteractionPrompt();
    }

    public void PrepareInteraction()
    {
        _actionText.ToggleInteractionPrompt();
        Player.Instance.SetInteractableObject(this);
    }

    public void ToggleDrawAttention()
    {
        Debug.Log("Drawn Attention");
        _actionText.ToggleWhiteDot();
    }

    bool IsPlayerAheadOfPos()
    {
        Vector3 playerDirection = (Player.Instance.transform.position - transform.position).normalized;
        Vector3 playerForward = Player.Instance.transform.forward;
        float dot = Vector3.Dot(playerDirection, transform.forward);

        return dot > 0;
    }

    void OpenDoor()
    {
        if (IsPlayerAheadOfPos())
        {
            _door.transform.DOLocalRotate(new Vector3(0, -90, 0), 1f).OnComplete(() => _isOpen = true);

        }

        else
        {
            _door.transform.DOLocalRotate(new Vector3(0, 90, 0), 1f).OnComplete(() => _isOpen = true);
        }

    }

    void CloseDoor()
    {
        _door.transform.DOLocalMoveY(0, 1f);

    }
}
