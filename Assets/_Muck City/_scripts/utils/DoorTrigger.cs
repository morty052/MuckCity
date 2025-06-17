using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
// using StarterAssets;
using System.Collections;
using UnityUtils;
using Invector.vCharacterController;
using Sirenix.OdinInspector;

public class DoorTrigger : MonoBehaviour, IInteractable
{
    [SerializeField] GameObject _door;

    [SerializeField] bool _isOpen = false;
    [SerializeField] bool _canInteract = true;

    public bool CanInteract => _canInteract;

    public string InteractionPrompt => "Open";



    void OnTriggerEnter(Collider other)
    {

        if (!other.CompareTag("InteractionHelper")) return;
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
        if (IsPlayerAheadOfPos())
        {
            _door.transform.DOLocalRotate(new Vector3(0, -90, 0), 1f).OnComplete(() => _isOpen = true);

        }

        else
        {
            _door.transform.DOLocalRotate(new Vector3(0, 90, 0), 1f).OnComplete(() => _isOpen = true);
        }

    }

    bool IsPlayerAheadOfPos()
    {
        Vector3 playerDirection = (Player.Instance.transform.position - transform.position).normalized;
        Vector3 playerForward = Player.Instance.transform.forward;
        float dot = Vector3.Dot(playerDirection, transform.forward);

        return dot > 0;
    }

    public void PrepareInteraction()
    {
        HudManager.Instance.ShowInteractPrompt(InteractionPrompt);
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
