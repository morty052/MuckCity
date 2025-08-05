using Invector.vItemManager;
using Sirenix.OdinInspector;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [HideInInspector] public vInventory _inventory;

    vItemManager _itemManager;

    [TabGroup("Storage")] public BackPack _hotStorage;
    [TabGroup("Storage")] public Storage _activeStorage;

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

    public void Start()
    {
        _inventory = Player.Instance.GetComponentInChildren<vInventory>();
        _itemManager = Player.Instance.GetComponent<vItemManager>();
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
        Debug.Log("Item added to inventory: " + item.name);

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
