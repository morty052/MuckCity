using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;
using System;

public interface IOnClickSlotReceiver
{
    void OnSlotClicked(string slotId);
}

public class SpecialEquipmentSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] Color _defaultColor;
    [SerializeField] Color _activeColor;
    [SerializeField] Image _backgroundImage;

    public string _slotId;

    public IOnClickSlotReceiver _onClickSlotReceiver;

    void Start()
    {
        // Preserve current alpha
        Color colorWithAlpha = new(
            _activeColor.r,
            _activeColor.g,
            _activeColor.b,
            _activeColor.a // Keep current alpha
        );

    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Mouse entered UI element!");
        _backgroundImage.DOColor(_activeColor, 0.2f);
        _backgroundImage.DOFade(1, 0.1f);
        _backgroundImage.transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.2f);

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Mouse exited UI element!");
        _backgroundImage.DOColor(_defaultColor, 0.2f).OnComplete(() => _backgroundImage.transform.DOScale(Vector3.one, 0.2f));

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Mouse clicked UI element!");
        _onClickSlotReceiver.OnSlotClicked(_slotId);
    }
}