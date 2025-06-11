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
        _text.text = shopItemSO._name;
        _image.sprite = shopItemSO._icon;
    }
}
