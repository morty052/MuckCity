using DG.Tweening;
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

    public void CompleteObjective()
    {
        // Debug.Log("trying to complete" + _text.text);
        transform.DOScale(0, 0.5f).OnComplete(() => { Destroy(gameObject); });
    }
}
