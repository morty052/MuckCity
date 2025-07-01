using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;


public interface IBrowsable
{
    public void OnButtonPress(Inputs button);
}

public class Rack : Interactable, IBrowsable
{

    [SerializeField] Camera _cam;
    [SerializeField] Camera _focusCam;
    [SerializeField] Transform _debugTransform;
    [SerializeField] private int _selectedItemIndex = 0;
    [SerializeField] private List<ShopItemSO> _itemSOs;
    [SerializeField] private List<Tradeable> _items;

    [SerializeField] GameObject UI;
    [SerializeField] TextMeshProUGUI _selectedItemNameText;
    [SerializeField] TextMeshProUGUI _selectedItemPriceText;

    int _columns = 3;
    int _rows = 2;

    void Awake()
    {
        for (int i = 0; i < _itemSOs.Count; i++)
        {
            ShopItemSO SO = _itemSOs[i];
            Tradeable tradeable = Instantiate(SO._tradeable, SO._rackPos.position, Quaternion.Euler(SO._rackPos.rotation), transform);
            _items.Add(tradeable);
        }

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
            default:
                break;
        }
        _items[_selectedItemIndex].ToggleHighlight();
        _selectedItemNameText.text = _items[_selectedItemIndex]._itemData._name;
        _selectedItemPriceText.text = _items[_selectedItemIndex]._itemData._price.ToString();
    }
}
