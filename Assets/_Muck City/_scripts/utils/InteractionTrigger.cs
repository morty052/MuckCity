using UnityEngine;
using UnityEngine.Events;

public class InteractionTrigger : MonoBehaviour
{

    // [SerializeField] UnityEvent OnInteract;
    [SerializeField] UnityEvent OnTriggerEnterEvent;
    [SerializeField] UnityEvent OnTriggerExitEvent;


    void OnTriggerEnter()
    {
        OnTriggerEnterEvent?.Invoke();
    }
    void OnTriggerExit()
    {
        OnTriggerExitEvent?.Invoke();
    }



}
