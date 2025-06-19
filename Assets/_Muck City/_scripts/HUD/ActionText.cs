using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionText : MonoBehaviour
{
    [SerializeField] Text _title;
    [SerializeField] GameObject _whiteDotImage;
    [SerializeField] GameObject _TextUi;

    public bool IsHighlighted { get => _whiteDotImage.activeSelf; }

    public void SetText(string text)
    {
        _title.text = text;
    }

    public void ToggleInteractionPrompt()
    {
        _whiteDotImage.SetActive(!_whiteDotImage.activeSelf);
        _TextUi.SetActive(!_TextUi.activeSelf);

    }

    public void ToggleWhiteDot()
    {
        _whiteDotImage.SetActive(!_whiteDotImage.activeSelf);
    }
}
