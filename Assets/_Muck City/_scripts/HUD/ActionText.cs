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
        _whiteDotImage.SetActive(false);
        _TextUi.SetActive(true);

    }
    public void ShowHideInteractionPrompt()
    {
        _TextUi.SetActive(!_TextUi.activeSelf);
    }
    public void HideInteractionPrompt()
    {
        _TextUi.SetActive(false);
        _whiteDotImage.SetActive(true);
    }
    public void ShowInteractionPrompt()
    {
        _TextUi.SetActive(true);
    }

    public void ToggleWhiteDot()
    {
        _whiteDotImage.SetActive(!_whiteDotImage.activeSelf);
    }
}
