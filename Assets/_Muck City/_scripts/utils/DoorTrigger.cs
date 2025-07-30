using UnityEngine;
using DG.Tweening;
using System.Collections;
using Sirenix.OdinInspector;
using System;

public enum Direction
{
    BACK,
    FRONT
}

public enum DoorTriggerType
{
    FLOOR,
    HANDLE
}

public class DoorTrigger : Interactable, IUseEnergy
{
    [SerializeField] GameObject _door;

    [SerializeField] bool _isOpen = false;

    [SerializeField] Direction _direction;
    [SerializeField] DoorTriggerType _type;
    [SerializeField, ShowIf("_type", DoorTriggerType.FLOOR)] DoorTrigger _doorHandle;
    [SerializeField, ShowIf("_type", DoorTriggerType.FLOOR)] DoorTrigger _oppositeDoorTrigger;
    [SerializeField, ShowIf("_type", DoorTriggerType.HANDLE)] DoorTrigger _backTrigger;
    [SerializeField, ShowIf("_type", DoorTriggerType.HANDLE)] DoorTrigger _frontTrigger;

    bool _isOccupied = false;

    public float EnergyLevel { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public bool IsPowered => throw new NotImplementedException();

    public float EnergyNeededToFunction => 0;

    public Transform ChargingPort => throw new NotImplementedException();

    // public Action<string> OnInteracted;


    public override void Start()
    {
        base.Start();
        if (_type == DoorTriggerType.HANDLE)
        {
            _interactionPrompt = "Close";
        }
    }

    void OnTriggerEnter(Collider other)
    {

        if (!_canInteract) return;
        Debug.Log(other.name + "entered trigger");
        Player.Instance.SetInteractableObject(this);
        PrepareInteraction();
        _isOccupied = true;
    }


    void OnTriggerExit(Collider other)
    {
        Player.Instance.SetInteractableObject(null);
        HideInteractionPrompt();
        _isOccupied = false;
    }

    void CloseDoor()
    {
        _canInteract = false;
        HideInteractionPrompt();
        _oppositeDoorTrigger._canInteract = false;
        _door.transform.DOLocalRotate(new Vector3(0, 0, 0), 1f).OnComplete(() =>
        {
            _isOpen = false;
            _canInteract = true;
            _oppositeDoorTrigger._canInteract = true;
            _oppositeDoorTrigger._isOpen = false;
            _interactionPrompt = "Open";
            if (_isOccupied)
            {
                PrepareInteraction();
            }
            if (_type == DoorTriggerType.HANDLE)
            {
                _frontTrigger.gameObject.SetActive(true);
                _backTrigger.gameObject.SetActive(true);
                gameObject.SetActive(false);
            }
        });

    }
    void OpenDoor()
    {
        _canInteract = false;
        HideInteractionPrompt();
        if (_direction == Direction.FRONT)
        {
            Debug.Log("Player is ahead of pos");
            _door.transform.DOLocalRotate(new Vector3(0, -90, 0), 1f).OnComplete(() => { _isOpen = true; _oppositeDoorTrigger._isOpen = true; _canInteract = true; });

        }

        else
        {
            Debug.Log("Player is behind of pos");
            _door.transform.DOLocalRotate(new Vector3(0, 90, 0), 1f).OnComplete(() => { _isOpen = true; _oppositeDoorTrigger._isOpen = true; _canInteract = true; });
        }

        // _doorHandle.GameObject.SetActive(true);
        // _oppositeDoorTrigger.GameObject.SetActive(false);
        // gameObject.SetActive(false);
        _interactionPrompt = "Close";
        if (_isOccupied)
        {
            PrepareInteraction();
        }
    }

    bool IsPlayerAheadOfPos()
    {
        Vector3 playerDirection = (Player.Instance.transform.position - _door.transform.position).normalized;
        float dot = Vector3.Dot(playerDirection, _door.transform.forward);
        Debug.Log("Dot is " + dot);
        return dot > 0;
    }

    public override void Interact()
    {
        if (_type == DoorTriggerType.HANDLE)
        {
            CloseDoor();
            return;
        }
        if (!_isOpen)
        {
            OpenDoor();
        }

        else
        {
            CloseDoor();
        }

        if (IsQuestItem)
        {
            QuestItem questItem = GetComponent<QuestItem>();
            OnInteracted?.Invoke(questItem._questItemData._tag);
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

    public void PowerUp(float amount)
    {
        _canInteract = true;
    }

    public void PromptToCharge()
    {
        throw new NotImplementedException();
    }

    public void OnChargeComplete()
    {
        throw new NotImplementedException();
    }

    public void PowerDown()
    {
        _canInteract = false;
    }


    // public virtual void ToggleCanInteract()
    // {
    //     _canInteract = !_canInteract;
    // }

}
