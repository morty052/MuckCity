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

    bool _isQuestItem;

    public bool IsQuestItem { get; set; }

    public void HideInteractionPrompt()
    {
        _actionText.HideInteractionPrompt();
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
        Debug.Log(other.name + " Entered trigger");
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log("Exited trigger");
        HideInteractionPrompt();
    }

    public void PrepareInteraction()
    {
        _actionText.ShowInteractionPrompt();
        Player.Instance.SetInteractableObject(this);
    }

    public void ToggleDrawAttention()
    {
        _actionText.ToggleWhiteDot();
    }

    bool IsPlayerAheadOfPos()
    {
        Vector3 playerDirection = (Player.Instance.transform.position - _door.transform.position).normalized;
        float dot = Vector3.Dot(playerDirection, _door.transform.forward);

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
