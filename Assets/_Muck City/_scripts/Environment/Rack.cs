using System;
using System.Collections.Generic;
using System.Linq;
using Invector.vItemManager;
using Sirenix.OdinInspector;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;


public interface IBrowsable
{
    public void OnButtonPress(Inputs button);
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

    [SerializeField] Camera _cam;
    [SerializeField] Camera _focusCam;
    [SerializeField] GameObject UI;
    [SerializeField] TextMeshProUGUI _selectedItemNameText;
    [SerializeField] TextMeshProUGUI _selectedItemPriceText;
    [SerializeField] private int _selectedItemIndex = 0;
    [SerializeField] private List<ShopItemSO> _itemSOs;
    [SerializeField] private List<Tradeable> _items;
    [SerializeField] Transform _debugTransform;



    [SerializeField] int _columns = 3;
    [SerializeField] int _rows = 3;

    void Awake()
    {
        for (int i = 0; i < _itemSOs.Count; i++)
        {
            ShopItemSO SO = _itemSOs[i];
            Tradeable tradeable = Instantiate(SO._tradeable, SO._rackPos.position, Quaternion.Euler(SO._rackPos.rotation), transform);
            tradeable.transform.SetLocalPositionAndRotation(SO._rackPos.position, Quaternion.Euler(SO._rackPos.rotation));
            _items.Add(tradeable);

        }

        //* ORDER ITEMS BY ORDER IN RACK TO GET THEM IN THE CORRECT ORDER
        _items = _items.OrderBy(x => x._itemData._orderInRack).ToList();
    }

    [Button("focus")]
    void LookAtWeapon()
    {
        _focusCam.transform.LookAt(_items[_selectedItemIndex].transform);
    }
    [Button("focus")]
    void DebugLookAtWeapon()
    {
        _focusCam.transform.LookAt(_debugTransform);
    }


    void OnTriggerEnter(Collider other)
    {
        PrepareInteraction();
    }
    void OnTriggerExit(Collider other)
    {
        HideInteractionPrompt();
    }




    [Button("Next Row")]
    int GetNextRow()
    {
        int currentRow = _selectedItemIndex / _columns + 1;


        bool isLastRow = currentRow == _rows;

        int nextRow = isLastRow ? 1 : currentRow + 1;

        int selectionIndexInNextRow = _selectedItemIndex % _columns + (_columns * (nextRow - 1));

        Debug.Log("row: " + currentRow + " last row: " + isLastRow + " next row: " + nextRow + " selection index: " + selectionIndexInNextRow);
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
            case Inputs.SELECT:
                break;
            case Inputs.BACK:
                break;
            case Inputs.EXIT:
                ExitRack();
                break;
            case Inputs.BUY:
                Buy();
                break;
            default:
                break;
        }
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
    }


}
