using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveItem : MonoBehaviour
{

    public TextMeshProUGUI _text;
    public Image _checkBoxSprite;

    public void SetupObjective(Objective objective)
    {
        _text.text = objective._title;
    }
}
