using UnityEngine;

public class Generator : Equipment
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Interact()
    {
        CanInteract = false;
        Debug.Log("Interacting with " + gameObject.name);
        _actionText.SetText("Turn Off");
        // _actionText.HideInteractionPrompt();
        CanInteract = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _actionText.HideInteractionPrompt();
        }
    }


    public bool PlayerInRange() => Vector3.Distance(transform.position, Player.Instance.transform.position) < 0.5f;

}
