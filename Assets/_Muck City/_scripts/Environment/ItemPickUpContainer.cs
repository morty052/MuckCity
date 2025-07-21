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
        //* DISABLE COLLIDER TO AVOID GETTING INTO COLLIDER LIST AGAIN
        GetComponent<Collider>().enabled = false;
        //* REMOVE SELF TO AVOID BUG WHEN DESTROYED
        InteractionSystem._closestInteractables.Remove(this);
        if (IsQuestItem)
        {
            QuestItem questItem = GetComponent<QuestItem>();
            OnInteracted?.Invoke(questItem._questItemData._tag);
        }
        Destroy(gameObject, 0.2f);
    }

    void PickUp()
    {
        switch (_pickupType)
        {
            case PickupType.ITEM:
                break;
            case PickupType.SPECIAL_EQUIPMENT:
                SpecialEquipmentManager.Instance.AddSpecialEquipment(_specialEquipment._id);
                break;
            default:
                break;
        }
        // InteractionSystem.InvokeOnPick(GameObject);
        // GetComponent<Collider>().enabled = false;


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
