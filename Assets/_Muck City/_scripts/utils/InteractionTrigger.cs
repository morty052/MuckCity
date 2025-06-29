using UnityEngine;
using UnityEngine.Events;

public class InteractionTrigger : MonoBehaviour
{

    [SerializeField] UnityEvent OnInteract;
    [SerializeField] UnityEvent OnTriggerEnterEvent;
    [SerializeField] UnityEvent OnTriggerExitEvent;
    IInteractable _parent;

    void Awake()
    {
        _parent = GetComponentInParent<IInteractable>();
    }

    void OnTriggerEnter()
    {
        OnTriggerEnterEvent?.Invoke();
    }
    void OnTriggerExit()
    {
        OnTriggerExitEvent?.Invoke();
    }

    public void Interact()
    {
        OnInteract?.Invoke();
    }

    public void PrepareInteraction()
    {
        _parent.PrepareInteraction();
    }

    public void HideInteractionPrompt()
    {
        _parent.HideInteractionPrompt();
    }
}
