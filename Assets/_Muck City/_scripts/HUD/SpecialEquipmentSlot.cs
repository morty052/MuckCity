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
    [SerializeField] Color _backgroundColor;
    [SerializeField] Image _icon;
    [SerializeField] Image _slotBackgroundImage;
    [SerializeField] Image _unUsedSlotImage;

    public string _slotId;

    public IOnClickSlotReceiver _onClickSlotReceiver;

    public bool _isInUse = false;

    public bool _debug;

    void Start()
    {
        _slotBackgroundImage.fillAmount = 0;
        // _unUsedSlotImage.gameObject.SetActive(true);
        // _icon.gameObject.SetActive(false);
    }

    public void Init(string slotId, IOnClickSlotReceiver onClickSlotReceiver, Sprite icon, Color backgroundColor = default)
    {
        _isInUse = true;
        _slotId = slotId;
        _onClickSlotReceiver = onClickSlotReceiver;
        _icon.sprite = icon;
        _icon.gameObject.SetActive(true);
        // _unUsedSlotImage.gameObject.SetActive(false);
        if (backgroundColor != default)
        {
            // _backgroundColor = backgroundColor;
            _slotBackgroundImage.color = _backgroundColor;
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_isInUse) return;
        Debug.Log("Mouse entered UI element!");
        _slotBackgroundImage.DOFillAmount(1, 0.2f).OnComplete(() => _slotBackgroundImage.transform.DOScale(Vector3.one * 1.1f, 0.1f)); ;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!_isInUse) return;
        _slotBackgroundImage.DOFillAmount(0, 0.2f).OnComplete(() => _slotBackgroundImage.transform.DOScale(Vector3.one, 0.1f));
        if (_debug)
        {
            Debug.Log("Mouse exited UI element!");
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!_isInUse) return;

        if (_onClickSlotReceiver != null)
        {
            _onClickSlotReceiver.OnSlotClicked(_slotId);
        }

        if (_debug)
        {

            Debug.Log("Mouse clicked UI element!");
        }
    }
}