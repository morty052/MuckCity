using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    public bool _isOpen = false;
    public Observer<bool> _isInteractionCalled = new(false);
    public float _speed = 1f;

    public Vector3 _startRotation;
    public Transform _highlightPos;

    [SerializeField] bool _canInteract = true;
    [SerializeField] string _interactionPrompt = "Open";

    public bool CanInteract => _canInteract;

    public GameObject GameObject => gameObject;

    public string InteractionPrompt => _interactionPrompt;

    public bool IsHighlighted => false;

    public bool IsQuestItem { get; set; }

    void Awake()
    {
        _startRotation = transform.rotation.eulerAngles;
    }

    [Button("Open")]
    public void Open()
    {
        _isInteractionCalled.Value = !_isInteractionCalled.Value;
        _canInteract = false;
        HideInteractionPrompt();
        Player.Instance.SetInteractableObject(null);
        float endRotation;
        if (IsPlayerAheadOfPos())
        {
            Debug.Log("Player is ahead of the door");
            endRotation = _startRotation.y + 90;
        }

        else
        {
            Debug.Log("Player is behind the door");
            endRotation = _startRotation.y - 90;
        }

        transform.DOLocalRotate(new Vector3(0, endRotation, 0), _speed).OnComplete(() =>
        {
            _isOpen = true;
            _interactionPrompt = "Close";
            _canInteract = true;
        });
    }

    void Close()
    {
        _isInteractionCalled.Value = !_isInteractionCalled.Value;
        _canInteract = false;
        HideInteractionPrompt();
        transform.DOLocalRotate(new Vector3(0, 0, 0), _speed).OnComplete(() =>
        {
            _isOpen = false;
            _interactionPrompt = "Open";
            _canInteract = true;
        });
    }

    void OnTriggerEnter(Collider other)
    {

        if (!_canInteract) return;
        Debug.Log(other.name + "entered trigger");
        PrepareInteraction();
    }

    void OnTriggerExit(Collider other)
    {
        HideInteractionPrompt();
        Player.Instance.SetInteractableObject(null);
    }

    bool IsPlayerAheadOfPos()
    {
        float dot = Vector3.Dot(transform.forward, (Player.Instance.transform.position - transform.position).normalized);
        Debug.Log("Dot is " + dot);
        return dot > 0;
    }

    public void ToggleDrawAttention()
    {

    }

    public void PrepareInteraction()
    {
        HudManager.Instance.ShowInteractPrompt(_highlightPos.position, InteractionPrompt);
        Player.Instance.SetInteractableObject(this);
    }

    public void Interact()
    {
        if (!_isOpen)
        {
            Open();
        }

        else
        {
            Close();
        }
    }

    public void HideInteractionPrompt()
    {
        HudManager.Instance.HideInteractPrompt();
    }
}
