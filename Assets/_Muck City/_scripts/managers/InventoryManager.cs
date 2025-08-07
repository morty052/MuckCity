using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Invector.vItemManager;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [HideInInspector] public vInventory _inventory;

    vItemManager _itemManager;

    [TabGroup("Storage")] public BackPack _hotStorage;
    [TabGroup("Storage")] public Storage _activeStorage;

    public static Action<ItemReference> OnAddItemToInventoryEvent;

    public TextMeshProUGUI _addedItemToInventoryText;

    List<ItemReference> _latestCollectedItems = new();

    [SerializeField] float _timeToDisplayNewItemText = 1f;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        GameEventsManager.OnCraftItemEvent += AddItemToInventory;
    }

    void OnDisable()
    {
        GameEventsManager.OnCraftItemEvent -= AddItemToInventory;
    }

    public void Init()
    {
        _inventory = Player.Instance.GetComponentInChildren<vInventory>();
        _itemManager = Player.Instance.GetComponent<vItemManager>();
    }

    // public void Start()
    // {
    //     _inventory = Player.Instance.GetComponentInChildren<vInventory>();
    //     _itemManager = Player.Instance.GetComponent<vItemManager>();
    // }

    void DisplayNewItemText(string item, int amount = 1)
    {
        string text = $"+{amount} {item}";
        _addedItemToInventoryText.text = text;
        FadeInText();
    }

    void FadeOutText()
    {
        _addedItemToInventoryText.DOFade(0f, 0.3f);
    }

    void FadeInText()
    {
        _addedItemToInventoryText.DOFade(1f, 0.3f);
    }
    async Task DisplayNewItems()
    {
        int itemCount = _latestCollectedItems.Count;
        while (itemCount > 0)
        {
            int index = itemCount - 1;
            Debug.Log("Item added to inventory: " + _latestCollectedItems[index].name + " amount: " + _latestCollectedItems[index].amount);
            DisplayNewItemText(_latestCollectedItems[index].name, _latestCollectedItems[index].amount);
            _latestCollectedItems.RemoveAt(index);
            itemCount--;
            await Task.Delay((int)(_timeToDisplayNewItemText * 1000));
            FadeOutText();
            // await Task.Yield();
        }
    }

    #region Inventory Usage

    public void CheckIfItemInInventory()
    {
        // _inventory.ContainItem();

    }
    public void UseItem(vItem item)
    {
        switch (item.type)
        {
            case vItemType.Consumable:
                Debug.Log("Consumable item: " + item.name);
                Instantiate(item.dropObject, transform.position, Quaternion.identity);
                break;
            case vItemType.ShooterWeapon:
                break;
            default:
                Debug.LogWarning("Item type not handled: " + item.type);
                break;
        }
    }
    public void EquipItem(vEquipArea equipArea, vItem item)
    {
        // switch (item.type)
        // {
        //     case vItemType.Consumable:
        //         Debug.Log("Consumable item: " + item.name);
        //         Instantiate(item.dropObject, transform.position, Quaternion.identity);
        //         break;
        //     case vItemType.ShooterWeapon:
        //         break;
        //     default:
        //         Debug.LogWarning("Item type not handled: " + item.type);
        //         break;
        // }
    }

    public void EquipBackPack(Transform backPack)
    {
        backPack.SetParent(Player.Instance._backPackHolder);
        backPack.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        BackPack b = backPack.GetComponent<BackPack>();
        _hotStorage = b;
        _activeStorage = b;

    }

    public void AddItemToInventory(ItemReference item)
    {
        _itemManager.AddItem(item);
        if (_hotStorage != null)
        {
            _hotStorage.AddItem(item);
        }
        OnAddItemToInventoryEvent?.Invoke(item);
    }
    public async void AddItemToInventory(List<ItemReference> _items)
    {
        //*PREPARE ITEMS FOR DISPLAY NOTIFICATION
        foreach (ItemReference item in _items)
        {
            //* ADD ITEM TO ACTUAL INVENTORY
            AddItemToInventory(new ItemReference(item.id));
            _latestCollectedItems.Add(item);
        }
        await DisplayNewItems();
    }




    public bool IsItemInInventory(int id)
    {
        if (_activeStorage == null)
        {
            return false;
        }
        bool hasItem = _activeStorage.IsItemInInventory(id);
        return hasItem;
    }



    #endregion
}
