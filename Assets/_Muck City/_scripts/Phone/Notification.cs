using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Notification : MonoBehaviour
{
    public Image _icon;
    // public TextMeshProUGUI _title;
    public TextMeshProUGUI _content;
    public TextMeshProUGUI _promptText;
    public GameObject _promptObject;
    [SerializeField] float _bottomPadding;

    [SerializeField] bool _debug;

    [Button]
    public void SetHeight()
    {
        int totalLines = _content.textInfo.lineCount + 1;
        float lineHeight = _content.textInfo.lineInfo[0].lineHeight;
        float totalLineheight = lineHeight * totalLines;

        float _iconHeight = _icon.GetComponent<RectTransform>().rect.height / 2;

        float padding = _bottomPadding + _iconHeight;

        if (_debug)
        {
            Debug.Log($"Line height: {totalLineheight}");
            Debug.Log($"Total lines: {totalLines}");
            Debug.Log($"icon height : {_iconHeight}");
            Debug.Log($"padding : {padding}");
        }

        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector2 sizeDelta = rectTransform.sizeDelta;
        sizeDelta.y = totalLineheight + _iconHeight;
        rectTransform.sizeDelta = sizeDelta;
    }

    public void SetText()
    {

    }


    public void SetNotification(Sprite icon, string content, string promptText = null)
    {
        if (icon != null)
        {
            _icon.sprite = icon;
        }
        _content.text = content;

        //* SHOW PROMPT WITH NOTIFICATION IF AVAILABLE
        if (promptText != null)
        {
            _promptText.text = promptText;
            _promptObject.SetActive(true);
        }

        gameObject.SetActive(true);
        //* SCALE IN
        gameObject.transform.DOScale(Vector3.one, 0.3f);
    }

    public void HideNotification(Action callBack = null)
    {
        //* STOPS EXTERNAL CANVAS FROM BEING HIDDEN WHEN NOTIFICATION IS CLOSED BEFORE ITS REGULAR SCREEN TIME
        if (!gameObject.activeSelf) return;
        callBack?.Invoke();
        gameObject.transform.DOScale(Vector3.zero, 0.3f).OnComplete(() =>
        {
            if (_promptObject.activeSelf)
            {
                _promptObject.SetActive(false);
            }
            gameObject.SetActive(false);

        });
    }
}
