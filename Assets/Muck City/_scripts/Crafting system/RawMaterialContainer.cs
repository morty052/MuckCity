using Invector.vItemManager;
using UnityEngine;

public class RawMaterialContainer : Tradeable, IInteractable, IPoolable
{
    [SerializeField] RawMaterialSO Data;
    [SerializeField] public ItemReference _reference;

    [SerializeField] bool _canInteract;
    [SerializeField] string _interactionPrompt;

    public bool CanInteract => _canInteract;

    public string InteractionPrompt => _interactionPrompt;

    public int ID => _reference.id;

    public GameObject GameObject => gameObject;

    public PoolID PoolID => Data._poolID;

    void Awake()
    {
        if (Data != null)
        {
            _reference = Data._ref;
        }
    }

    public void HideInteractionPrompt()
    {
        HudManager.Instance.HideInteractPrompt();
    }

    public void Interact()
    {
        PickUp();
    }

    public void PickUp()
    {
        Player.Instance.AddItemToInventory(_reference);
        HideInteractionPrompt();
        Destroy(gameObject);
    }

    public void PrepareInteraction()
    {
        HudManager.Instance.ShowInteractPrompt("Pick Up");
    }
    public void PrepareInteraction(GameObject obj)
    {
        HudManager.Instance.ShowInteractPrompt("Pick Up");
    }

    public void SetData(RawMaterialSO data = null)
    {
        if (data != null)
        {
            Data = data;
        }
        _reference = Data._ref;
    }
}
