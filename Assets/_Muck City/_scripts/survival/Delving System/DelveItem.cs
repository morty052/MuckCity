using UnityEngine;

public class DelveItem : Interactable
{
    public string _id;
    public override void Start()
    {
        base.Start();
        _interactionPrompt = "Retrieve";
    }

    public override void Interact()
    {
        Debug.Log("Retrieving Delve Item");
        DelveManager.Instance.OnRetrieveDelveItem(_id);
    }

    void OnTriggerEnter(Collider other)
    {
        PrepareInteraction();
    }

    void OnTriggerExit(Collider other)
    {
        HideInteractionPrompt();
    }
}
