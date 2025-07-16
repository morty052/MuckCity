using TMPro;
using UnityEngine;

public class ActionPrompt : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _promptOne;
    [SerializeField] TextMeshProUGUI _promptTwo;

    public void UseActionPrompt(string promptOne, string promptTwo = null)
    {
        // _externalCanvas.SetActive(true);
        // _actionPrompt.gameObject.SetActive(true);
        _promptOne.text = promptOne;
        if (!string.IsNullOrEmpty(promptTwo))
        {
            _promptTwo.text = promptTwo;
        }

        else
        {
            _promptTwo.gameObject.SetActive(false);
        }
    }

}
