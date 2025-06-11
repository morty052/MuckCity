using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemButton : MonoBehaviour
{
    public Button _button;
    public TextMeshProUGUI _text;

    public Image _image;

    public void InitVisuals(ShopItemSO shopItemSO)
    {
        _text.text = shopItemSO._name;
        _image.sprite = shopItemSO._icon;
    }

}
