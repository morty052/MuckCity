using Sirenix.OdinInspector;
using UnityEngine;
using System;

public enum PickupType
{
    ITEM = 0,
    SPECIAL_EQUIPMENT = 1
}

public class ItemPickUpContainer : Interactable
{

    public PickupType _pickupType;

    [ShowIf("_pickupType", PickupType.SPECIAL_EQUIPMENT)]
    public SpecialEquipmentSO _specialEquipment;

    // public bool CanInteract => true;

    // public GameObject GameObject => gameObject;

    // public string InteractionPrompt => "Pick Up";

    // public bool IsHighlighted => _actionText.IsHighlighted;

    // [SerializeField] bool _isQuestItem;

    // public bool IsQuestItem { get => _isQuestItem; set => _isQuestItem = value; }

    // public Action<string> OnInteracted;





    public override void Interact()
    {
        PickUp();
        if (IsQuestItem)
        {
            QuestItem questItem = GetComponent<QuestItem>();
            OnInteracted?.Invoke(questItem._questItemData._tag);
        }
    }

    void PickUp()
    {
        switch (_pickupType)
        {
            case PickupType.ITEM:
                break;
            case PickupType.SPECIAL_EQUIPMENT:
                Player.Instance.AddSpecialEquipment(_specialEquipment._id);
                break;
            default:
                break;
        }
    }

    // public void HideInteractionPrompt()
    // {
    //     _actionText.HideInteractionPrompt();
    //     Player.Instance.SetInteractableObject(null);
    // }

    // public void PrepareInteraction()
    // {
    //     _actionText.ShowInteractionPrompt();
    //     Player.Instance.SetInteractableObject(this);
    // }

    // public void ToggleDrawAttention()
    // {
    //     _actionText.ToggleWhiteDot();
    // }

    // public void SetupInteractionListener(Action<string> action)
    // {
    //     throw new NotImplementedException();
    // }
}
