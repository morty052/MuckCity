using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public interface IBrowsable
{
    public void OnButtonPress(Inputs button);
}

[Serializable]
public struct RackItem
{
    [HideLabel, HorizontalGroup("Left")]
    public ShopItemSO SO;
    [HorizontalGroup("Left"), HideLabel]
    public int _orderInRack;

    public Pos transform;
    public Pos _previewTransform;

    public RackItem(ShopItemSO SO, int _orderInRack, Pos transform, Pos previewTransform)
    {
        this.SO = SO;
        this._orderInRack = _orderInRack;
        this.transform = transform;
        this._previewTransform = previewTransform;
    }
}
public interface IFindable
{
    public GameObject GameObject { get; }
    public bool IsQuestItem { get; set; }
    public void SetupInteractionListener(Action<string> action);
    public void RemoveInteractionListener(Action<string> action);
}

public class Rack : Interactable, IBrowsable
{

    [TabGroup("Components")]
    [SerializeField] Camera _cam;
    [TabGroup("Components")]
    [SerializeField] Camera _inspectorCam;
    [TabGroup("Components")]
    [SerializeField] private Transform _inspectionSpot;
    [TabGroup("Components")]
    [SerializeField] GameObject UI;
    [TabGroup("Components")]
    [SerializeField] GameObject _inspectionUI;
    [TabGroup("Components")]
    [SerializeField] TextMeshProUGUI _selectedItemNameText;
    [TabGroup("Components")]
    [SerializeField] TextMeshProUGUI _selectedItemPriceText;

    [TabGroup("Settings")]
    [SerializeField] private List<RackItem> _itemSOs;
    [TabGroup("Settings")]
    public ShopItemType _stockType;

    [TabGroup("Settings")]
    [SerializeField] int _columns = 3;
    [TabGroup("Settings")]
    [SerializeField] int _rows = 3;
    [TabGroup("Settings")]
    [SerializeField] private bool _orderByOrderInRack = true;

    [TabGroup("Settings")]
    [SerializeField] private float _smallItemZoomOffset;

    [TabGroup("Debug")]
    [SerializeField] private bool _debug = false;

    [TabGroup("Debug")]
    [SerializeField] private List<Tradeable> _items;

    [TabGroup("Debug")]
    [SerializeField] Transform _debugTransform;

    [TabGroup("Debug")]
    [SerializeField] private int _selectedItemIndex = 0;
    [TabGroup("Debug")]
    [SerializeField] private bool _isInspectingItem = false;

    [TabGroup("Debug")]
    [SerializeField] private GameObject _inspectedItem = null;


    [TabGroup("Events")]
    [SerializeField] UnityEvent<ShopItemType> OnInitialized;

    [TabGroup("Events")]
    [SerializeField] UnityEvent<ShopItemSO> OnChangeSelection;

    [TabGroup("Events")]
    [SerializeField] UnityEvent<ShopItemSO> OnStartInspectItem;
    [TabGroup("Events")]
    [SerializeField] UnityEvent OnExitInspectItem;


    void Awake()
    {
        if (_orderByOrderInRack)
        {
            _itemSOs = _itemSOs.OrderBy(x => x._orderInRack).ToList();
        }
        for (int i = 0; i < _itemSOs.Count; i++)
        {
            ShopItemSO SO = _itemSOs[i].SO;
            Tradeable tradeable = Instantiate(SO._tradeable, _itemSOs[i].transform.position, Quaternion.Euler(_itemSOs[i].transform.rotation), transform);
            tradeable.transform.SetLocalPositionAndRotation(_itemSOs[i].transform.position, Quaternion.Euler(_itemSOs[i].transform.rotation));
            _items.Add(tradeable);
        }

        // //* ORDER ITEMS BY ORDER IN RACK TO GET THEM IN THE CORRECT ORDER
        // _items = _items.OrderBy(x => x._itemData._orderInRack).ToList();
    }

    public override void Start()
    {
        base.Start();
        OnInitialized?.Invoke(_stockType);
        if (_items.Count != 0)
        {
            OnChangeSelection?.Invoke(_items[_selectedItemIndex]._itemData);
        }
    }


    void OnTriggerEnter(Collider other)
    {
        PrepareInteraction();
    }
    void OnTriggerExit(Collider other)
    {
        HideInteractionPrompt();
    }

    [Button("Next Row"), TabGroup("Debug")]
    int GetNextRow()
    {
        int currentRow = _selectedItemIndex / _columns + 1;


        bool isLastRow = currentRow == _rows;

        int nextRow = isLastRow ? 1 : currentRow + 1;

        int selectionIndexInNextRow = _selectedItemIndex % _columns + (_columns * (nextRow - 1));

        if (_debug)
        {
            Debug.Log("row: " + currentRow + " last row: " + isLastRow + " next row: " + nextRow + " selection index: " + selectionIndexInNextRow);
        }
        return selectionIndexInNextRow;
    }

    private int GetPreviousRow()
    {
        int currentRow = _selectedItemIndex / _columns + 1;
        bool isFirstRow = currentRow == 1;
        int previousRow = isFirstRow ? _rows : currentRow - 1;
        int selectionIndexInPreviousRow = _selectedItemIndex % _columns + (_columns * (previousRow - 1));
        Debug.Log("row: " + currentRow + " last row: " + isFirstRow + " next row: " + previousRow + " selection index: " + selectionIndexInPreviousRow);
        return selectionIndexInPreviousRow;
    }

    public override void Interact()
    {
        _cam.gameObject.SetActive(true);
        HideInteractionPrompt();
        GameEventsManager.Instance.OnToggleUi();
        Player.Instance.UseAltControls(true, this);
        UI.SetActive(true);
        _items[_selectedItemIndex].ToggleHighlight();
        _selectedItemNameText.text = _items[_selectedItemIndex]._itemData._name;
        _selectedItemPriceText.text = _items[_selectedItemIndex]._itemData._price.ToString();
    }

    public void OnButtonPress(Inputs input)
    {
        switch (input)
        {
            case Inputs.LEFT:
                HandleNavigation(input);
                break;
            case Inputs.RIGHT:
                HandleNavigation(input);
                break;
            case Inputs.UP:
                HandleNavigation(input);
                break;
            case Inputs.DOWN:
                HandleNavigation(input);
                break;
            case Inputs.INSPECT:
                InspectItem();
                break;
            case Inputs.EXIT:
                if (_isInspectingItem)
                {
                    _inspectorCam.gameObject.SetActive(false);
                    _inspectionUI.SetActive(false);
                    UI.SetActive(true);
                    _isInspectingItem = false;
                    Destroy(_inspectedItem);
                    OnExitInspectItem?.Invoke();
                    return;
                }
                ExitRack();
                break;
            case Inputs.BUY:
                Buy();
                break;
            default:
                break;
        }
    }

    [Button("Inspect"), TabGroup("Debug")]
    public void InspectItem()
    {

        UI.SetActive(false);
        _isInspectingItem = true;
        GameObject itemToInspect = Instantiate(_items[_selectedItemIndex]._itemData._tradeable.gameObject, _inspectionSpot);
        itemToInspect.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        // *Set item to close up layer
        itemToInspect.layer = LayerMask.NameToLayer("CloseUp");

        // *Set All children of item to close up layer
        foreach (Transform child in itemToInspect.transform)
        {
            child.gameObject.layer = LayerMask.NameToLayer("CloseUp");
        }
        //* Check if item is small
        bool isSmallItem = IsSmallItem(itemToInspect);
        if (isSmallItem)
        {
            // *Zoom in small items
            itemToInspect.transform.localPosition = new Vector3(_smallItemZoomOffset, 0, 0);
        }
        _inspectionUI.SetActive(true);
        _inspectedItem = itemToInspect;
        _inspectorCam.gameObject.SetActive(true);

        if (_debug)
        {
            Debug.Log("Inspecting " + _items[_selectedItemIndex]._itemData._name + " Is Small Item " + IsSmallItem(itemToInspect));
        }
        OnStartInspectItem?.Invoke(_items[_selectedItemIndex]._itemData);
    }


    bool IsSmallItem(GameObject item)
    {
        MeshFilter meshFilter = item.GetComponentInChildren<MeshFilter>();
        if (_debug)
        {

            Debug.Log("Size Z: " + meshFilter.sharedMesh.bounds.size.z);
        }
        if (meshFilter != null)
        {
            return meshFilter.sharedMesh.bounds.size.z < 0.7f;
        }
        return false;
    }

    private void ExitRack()
    {
        _items[_selectedItemIndex].ToggleHighlight();
        _selectedItemIndex = 0;
        _cam.gameObject.SetActive(false);
        // Player.Instance.ToggleModel();
        Player.Instance.UseAltControls(false);
        UI.SetActive(false);
        GameEventsManager.Instance.OnToggleUi();
    }

    public void Buy()
    {
        Tradeable item = _items[_selectedItemIndex]; //get item
        if (SocialCreditManager.Instance.CanBuy(item._itemData._price))
        {
            item.OnBuy(item._itemData);
            GameEventsManager.Instance.OnBuyItem(item._itemData);
            if (IsQuestItem)
            {
                QuestItem questItem = GetComponent<QuestItem>();
                OnInteracted?.Invoke(questItem._questItemData._tag);
            }
        }
        else
        {
            Debug.Log("Not enough deniro");
        }
    }

    void HandleNavigation(Inputs input)
    {
        _items[_selectedItemIndex].ToggleHighlight();
        switch (input)
        {
            case Inputs.LEFT:
                // MOVE TO PREVIOUS APP IF CAN GO BACK
                if (_selectedItemIndex > 0)
                {
                    _selectedItemIndex--;
                }
                else
                {
                    //MOVE TO LAST APP IF CANT GO BACK
                    _selectedItemIndex = _items.Count - 1;
                }
                break;
            case Inputs.RIGHT:
                if (_selectedItemIndex == _items.Count - 1)
                {
                    //MOVE TO FIRST APP IF CANT GO FORWARD
                    _selectedItemIndex = 0;
                }

                else
                {
                    //MOVE TO NEXT APP IF CAN GO FORWARD
                    _selectedItemIndex++;
                }
                break;
            case Inputs.UP:
                _selectedItemIndex = GetPreviousRow();
                break;
            case Inputs.DOWN:
                _selectedItemIndex = GetNextRow();
                break;
            default:
                break;
        }
        _items[_selectedItemIndex].ToggleHighlight();
        _selectedItemNameText.text = _items[_selectedItemIndex]._itemData._name;
        _selectedItemPriceText.text = _items[_selectedItemIndex]._itemData._price.ToString();
        OnChangeSelection.Invoke(_items[_selectedItemIndex]._itemData);

        //* UPDATE INSPECTOR UI IF ALREADY INSPECTING ITEM
        if (_isInspectingItem)
        {
            if (_inspectedItem != null)
            {
                Destroy(_inspectedItem);
            }
            GameObject itemToInspect = Instantiate(_items[_selectedItemIndex]._itemData._tradeable.gameObject, _inspectionSpot);
            itemToInspect.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            // *Set item to close up layer
            itemToInspect.layer = LayerMask.NameToLayer("CloseUp");

            // *Set All children of item to close up layer
            foreach (Transform child in itemToInspect.transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("CloseUp");
            }
            //* Check if item is small
            bool isSmallItem = IsSmallItem(itemToInspect);
            if (isSmallItem)
            {
                // *Zoom in small items
                itemToInspect.transform.localPosition = new Vector3(_smallItemZoomOffset, 0, 0);
            }
            _inspectedItem = itemToInspect;
        }
    }



    // Pos GetRackItemPreviewPos(string name)
    //     {
    //         int index = _itemSOs.FindIndex(x => x.SO._tradeable._itemData._name == name);
    //         return _itemSOs[index]._previewTransform;
    //     }

    //     [Button("focus"), TabGroup("Debug")]
    //     void LookAtWeapon()
    //     {
    //         _inspectorCam.transform.LookAt(_items[_selectedItemIndex].transform);
    //     }
    //     [Button("focus"), TabGroup("Debug")]
    //     void DebugLookAtWeapon()
    //     {
    //         _inspectorCam.transform.LookAt(_debugTransform);
    //     }

}
