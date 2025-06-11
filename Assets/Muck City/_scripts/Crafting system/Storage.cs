using UnityEngine;
using System.Collections.Generic;
using Invector.vItemManager;


public enum StorageType
{
   STATIC,
   MOBILE,
   CAN_BE_EQUIPPED
}

public class Storage : MonoBehaviour
{

   public StorageType _storageType;



   [SerializeField] GameObject _interactionCanvas;

   [SerializeField] List<ItemReference> _items = new();



   public void DisplayCanvas()
   {
      _interactionCanvas.SetActive(!_interactionCanvas.activeSelf);
   }


   public virtual void Interact()
   {

   }

   public virtual void AddItem(ItemReference item)
   {
      _items.Add(item);
      Debug.Log("Player added " + item.name);
   }

   public bool IsItemInInventory(int id)
   {
      ItemReference item = _items.Find(x => x.id == id);
      if (item != null)
      {
         return true;
      }
      return false;
   }



   public void ShowInteractionPrompt()
   {
      HudManager.Instance.ShowInteractPrompt("Pick Up");
   }
   public void HideInteractionPrompt()
   {
      HudManager.Instance.HideInteractPrompt();
   }
}
