using UnityEngine;
using System.Collections.Generic;
using Invector.vItemManager;

public class HarvestableObject : MonoBehaviour, IHarvestableObject
{
    public GameObject GameObject => gameObject;


    public bool CanHarvest => true;

    public List<ItemReference> _items = new();


    public void OnHarvest()
    {
        gameObject.SetActive(false);
        // for (int i = 0; i < _items.Count; i++)
        // {
        //     InventoryManager.Instance.AddItemToInventory(_items[i]);
        // }
        InventoryManager.Instance.AddItemToInventory(_items);
    }
}

