using UnityEngine;
using DG.Tweening;
using System.Collections;
using Sirenix.OdinInspector;

public enum Direction
{
    BACK,
    FRONT
}

public class DoorTrigger : MonoBehaviour, IInteractable
{
    [SerializeField] GameObject _door;

    [SerializeField] bool _isOpen = false;
    [SerializeField] bool _canInteract = true;

    public bool CanInteract => _canInteract;

    public string InteractionPrompt => "Open";

    public GameObject GameObject => gameObject;

    public bool IsHighlighted { get; }

    bool _isQuestItem;

    public bool IsQuestItem { get; set; }

    [SerializeField] Direction _direction;

    void OnTriggerEnter(Collider other)
    {

        Debug.Log(other.name + "entered trigger");
        Player.Instance.SetInteractableObject(this);
        PrepareInteraction();
    }


    void OnTriggerExit(Collider other)
    {
        Player.Instance.SetInteractableObject(null);
        HideInteractionPrompt();
    }

    void CloseDoor()
    {
        _door.transform.DOLocalMoveY(0, 1f);

    }
    void OpenDoor()
    {
        if (_direction == Direction.FRONT)
        {
            Debug.Log("Player is ahead of pos");
            _door.transform.DOLocalRotate(new Vector3(0, -90, 0), 1f).OnComplete(() => _isOpen = true);

        }

        else
        {
            Debug.Log("Player is behind of pos");
            _door.transform.DOLocalRotate(new Vector3(0, 90, 0), 1f).OnComplete(() => _isOpen = true);
        }

    }

    bool IsPlayerAheadOfPos()
    {
        Vector3 playerDirection = (Player.Instance.transform.position - _door.transform.position).normalized;
        float dot = Vector3.Dot(playerDirection, _door.transform.forward);
        Debug.Log("Dot is " + dot);
        return dot > 0;
    }

    public void PrepareInteraction()
    {
        HudManager.Instance.ShowInteractPrompt(InteractionPrompt);
    }

    public void ToggleDrawAttention()
    {

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


    [Button]
    void OpenClose()
    {
        if (_isOpen)
        {
            _door.transform.localRotation = Quaternion.Euler(0, 0, 0);
            _isOpen = false;
        }

        else
        {
            _door.transform.localRotation = Quaternion.Euler(0, -90, 0);
            _isOpen = true;
        }
    }


    public void HideInteractionPrompt()
    {
        HudManager.Instance.HideInteractPrompt();
    }
}
