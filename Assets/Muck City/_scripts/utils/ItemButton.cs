using System;
using TMPro;
using UnityEngine;

public class ItemButton : MonoBehaviour
{
    public TextMeshProUGUI _itemNameText;

    public int _quantity;

    internal void Setup(string name, int quantity)
    {
        _itemNameText.text = $"{name}";
        _quantity = quantity;
    }

    public void ToggleActive(bool state)
    {
        if (state)
        {
            _itemNameText.color = Color.green;
        }

        else
        {
            _itemNameText.color = Color.white;
        }
    }
}
