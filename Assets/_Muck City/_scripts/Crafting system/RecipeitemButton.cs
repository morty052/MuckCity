using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeitemButton : MonoBehaviour
{
    public Button _button;
    public TextMeshProUGUI _text;

    public Image _image;

    public void InitVisuals(RecipeSO shopItemSO)
    {
        if (shopItemSO == null) return;
        _text.text = shopItemSO._name;
        _image.sprite = shopItemSO._icon;
    }
}
