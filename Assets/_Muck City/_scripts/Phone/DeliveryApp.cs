using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityUtils;

public class DeliveryApp : PhoneApp
{

    [SerializeField] int _pendingDeliveries = 0;

    public int _selectedPreviewIndex = 0;

    [SerializeField] List<DeliveryDisplayButton> _storedPreviews = new();
    [SerializeField] private TextMeshProUGUI _pendingDeliveriesText;

    [SerializeField] AppScreen _deliveryInfoPage;
    [SerializeField] TextMeshProUGUI _deliveryPriceText;

    [SerializeField] GameObject _deliveryPreviewsParent;
    [SerializeField] DeliveryDisplayButton _deliveryPreviewPrefab;

    [TabGroup("Debug")]
    public DeliveryData? _currentDelivery;

    public override void Init()
    {
        base.Init();
        GameEventsManager.OnDeliveryAddedEvent += HandleNewDelivery;
        GameEventsManager.OnDeliveryPointReachedEvent += OnDeliveryCompleted;
        Debug.Log($"<color=orange> Delivery App Started </color>");
    }


    void OnDisable()
    {
        GameEventsManager.OnDeliveryPointReachedEvent -= OnDeliveryCompleted;
        GameEventsManager.OnDeliveryAddedEvent -= HandleNewDelivery;
    }

    void AcceptDelivery()
    {
        GameEventsManager.Instance.OnDeliveryAccepted(_currentDelivery.Value);
    }

    private void OnDeliveryCompleted(DeliveryData data)
    {
        DeliveryDisplayButton deliveryDisplayButton = _storedPreviews.Find(x => x._data._deliveryId == data._deliveryId);
        Transform instanceToRemove = _deliveryPreviewsParent.transform.GetChild(_storedPreviews.FindIndex(x => x._data._deliveryId == data._deliveryId));
        Destroy(instanceToRemove.gameObject);
        _storedPreviews.Remove(deliveryDisplayButton);
        _pendingDeliveries--;
        _pendingDeliveriesText.text = _pendingDeliveries.ToString();
    }

    public override void OnUpPressed()
    {
        // MOVE TO PREVIOUS APP IF CAN GO BACK
        if (_selectedPreviewIndex > 0)
        {
            _selectedPreviewIndex--;
        }
        else
        {
            //MOVE TO LAST APP IF CANT GO BACK
            _selectedPreviewIndex = _storedPreviews.Count - 1;
        }
    }

    public override void OnDownPressed()
    {
        if (_selectedPreviewIndex == _storedPreviews.Count - 1)
        {
            //MOVE TO FIRST APP IF CANT GO FORWARD
            _selectedPreviewIndex = 0;
        }

        else
        {
            //MOVE TO NEXT APP IF CAN GO FORWARD
            _selectedPreviewIndex++;
        }
    }

    public override void OnSelectPressed()
    {
        if (_storedPreviews.Count == 0) return;
        Debug.Log($"selected preview fee is {_storedPreviews[_selectedPreviewIndex]._deliveryFee}");
        DeliveryDisplayButton delivery = _storedPreviews[_selectedPreviewIndex];
        _deliveryPriceText.text = delivery._deliveryFee.ToString();
        _deliveryInfoPage.gameObject.SetActive(true);
    }
    public override void OnAcceptPressed()
    {
        Debug.Log($"selected preview fee is {_storedPreviews[_selectedPreviewIndex]._deliveryFee}");
        GameEventsManager.Instance.OnDeliveryAccepted(_storedPreviews[_selectedPreviewIndex]._data);
        if (_deliveryInfoPage.gameObject.activeSelf)
        {
            _deliveryInfoPage.gameObject.SetActive(false);
        }

    }

    public override void OnBackPressed()
    {
        if (_deliveryInfoPage.gameObject.activeSelf)
        {
            _deliveryInfoPage.gameObject.SetActive(false);
        }

        else
        {
            base.OnBackPressed();
        }
    }

    public override void DoQuickAction()
    {
        QuickInspectDelivery();
    }

    private void ResetDeliveryState()
    {
        // _canQuickAcceptReject = false;
        _currentDelivery = null;
        Player.Instance._activeQuickAction = null;
    }
    public void QuickInspectDelivery()
    {
        // _canQuickAcceptReject = false;
        _notificationSystem.HideNotification();
        if (_currentDelivery.Value._chat != null)
        {
            Phone.Instance.OnReceiveInstantMessage?.Invoke(_currentDelivery.Value._chat);
            Phone.Instance.StartInstantMessage();
        }
    }



    public void HandleNewDelivery(DeliveryData data)
    {
        Debug.Log($"<color=orange> Delivery Received </color>");
        _currentDelivery = data;
        _pendingDeliveries++;
        _pendingDeliveriesText.text = _pendingDeliveries.ToString();

        DeliveryDisplayButton preview = Instantiate(_deliveryPreviewPrefab, _deliveryPreviewsParent.transform);
        preview.Init(data);
        _storedPreviews.Add(preview);
        Player.Instance._activeQuickAction = this;

        _notificationSystem.ShowNotification("New order", data._deliveryFee.ToString(), "Reply", ResetDeliveryState);
    }


}
