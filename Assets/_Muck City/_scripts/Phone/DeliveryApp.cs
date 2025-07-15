using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
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

    bool IsViewingDelivery => _deliveryInfoPage.gameObject.activeSelf;



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

    #region "overrides"
    public override void OnInit()
    {

        GameEventsManager.OnDeliveryAddedEvent += HandleNewDelivery;
        GameEventsManager.OnDeliveryPointReachedEvent += OnDeliveryCompleted;
        if (_debug)
        {
            Debug.Log($"<color=orange> Delivery App Started </color>");
        }
    }

    public override void DoQuickAction()
    {
        QuickInspectDelivery();
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
            //*MOVE TO FIRST APP IF CANT GO FORWARD
            _selectedPreviewIndex = 0;
        }

        else
        {
            //*MOVE TO NEXT APP IF CAN GO FORWARD
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
        if (_debug)
        {
            Debug.Log($"selected preview fee is {_storedPreviews[_selectedPreviewIndex]._deliveryFee}");
        }
        if (IsViewingDelivery)
        {
            GameEventsManager.Instance.OnDeliveryAccepted(_storedPreviews[_selectedPreviewIndex]._data);
            DisposeActionPrompt(true);
            // _deliveryInfoPage.gameObject.SetActive(false);
            // AcceptDelivery();
        }
    }
    public override void OnRejectPressed()
    {
        if (_debug)
        {
            Debug.Log($"selected preview fee is {_storedPreviews[_selectedPreviewIndex]._deliveryFee}");
        }
        if (IsViewingDelivery)
        {
            HidePhone();
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

    public override void OnDisablePhone()
    {
        GameEventsManager.OnDeliveryPointReachedEvent -= OnDeliveryCompleted;
        GameEventsManager.OnDeliveryAddedEvent -= HandleNewDelivery;
        if (_debug)
        {
            Debug.Log("Delivery App Disabled");
        }
    }
    #endregion

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
        _deliveryInfoPage.gameObject.SetActive(true);
        Phone.Instance.SetApp(ID, true);
        DisplayActionPrompt("Accept", "Ignore");
        // if (_currentDelivery.Value._chat != null)
        // {
        //     Phone.Instance.OnReceiveInstantMessage?.Invoke(_currentDelivery.Value._chat);
        //     Phone.Instance.StartInstantMessage();
        // }
    }

    public void HandleNewDelivery(DeliveryData data)
    {
        if (_debug)
        {
            Debug.Log($"<color=orange> Delivery Received </color>");
        }

        //* MAKE DELIVERY CURRENT DELIVERY FOR QUICK INSPECT 
        _currentDelivery = data;

        //* CREATE PREVIEW FOR DELIVERY
        DeliveryDisplayButton preview = Instantiate(_deliveryPreviewPrefab, _deliveryPreviewsParent.transform);
        preview.Init(data);

        //* STORE DELIVERY PREVIEW
        _storedPreviews.Add(preview);

        //* MAKE PREVIEW BUTTON OPEN CORRESPONDING DELIVERY ON CLICK
        CreateButton(preview, _storedPreviews.Count - 1);

        //* SET DELIVERY COUNT TEXT TO LENGTH TO OD PENDING DELIVERIES
        _pendingDeliveriesText.text = _storedPreviews.Count.ToString();


        //* ACTIVATE QUICK ACTION SO USER CAN QUICK INSPECT DELIVERY
        UseQuickAction();

        //* SHOW NOTIFICATION AND DISABLE QUICK QUICK INSPECT WHEN IT IS HIDDEN
        _notificationSystem.ShowNotification(AppIcon.IconSprite, $"New order {data._deliveryFee} Credits", "View", ResetDeliveryState);
    }

    private void CreateButton(DeliveryDisplayButton image, int index)
    {
        Button button = image.GetComponent<Button>();
        button.onClick.AddListener(() => { ExpandDeliveryPreview(index); });
    }

    void ExpandDeliveryPreview(int previewIndex)
    {
        DeliveryDisplayButton btn = _storedPreviews[previewIndex];
        DeliveryData deliveryData = btn._data;

        _deliveryPriceText.text = deliveryData._deliveryFee.ToString();

        _deliveryInfoPage.gameObject.SetActive(true);
    }
}
