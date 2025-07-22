using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityUtils;

public enum PropertyID
{
    LOT_A = 0,
    LOT_B = 1
}

public class PropertyInterface : Interactable, IBrowsable
{
    [SerializeField] List<Residence> _property = new();

    public GameObject _interfaceUI;

    public Residence PlayerLot => _property.Find(x => x._isPlayerOwned);

    [SerializeField] Residence _activeProperty;

    [SerializeField] GameObject _apartmentsParentUI;
    [SerializeField] GameObject _activePropertyUI;
    [SerializeField] GameObject _propertyControlPanel;

    [SerializeField] TextMeshProUGUI _propertyName;
    [SerializeField] TextMeshProUGUI _propertyRent;
    [SerializeField] TextMeshProUGUI _maxResidents;
    [SerializeField] TextMeshProUGUI _propertyEnergyCost;

    [SerializeField] GameObject _popUpsParent;

    bool IsPopupActive => _popUpsParent.transform.Children().Any(x => x.gameObject.activeSelf);
    Transform ActivePopup => _popUpsParent.transform.Children().FirstOrDefault(x => x.gameObject.activeSelf);

    public void TransferPropertyToPlayer(Residence property)
    {
        property.TransferToPlayer();
    }
    public void BuyProperty()
    {
        if (SocialCreditManager.Instance.CanBuy(_activeProperty._rent))
        {
            _activeProperty.TransferToPlayer();
            SocialCreditManager.Instance.Pay(_activeProperty._rent);
            Debug.Log("Buying Property");
            if (IsQuestItem)
            {
                QuestItem questItem = GetComponent<QuestItem>();
                OnInteracted?.Invoke(questItem._questItemData._tag);
            }
        }

        else
        {
            Debug.Log("Not Enough Dinero Holmes");
        }

    }


    public void PowerDownProperty(Residence property)
    {
        foreach (var energyConsumer in property._energyConsumers)
        {
            energyConsumer.PowerDown();
        }
    }
    public void PowerUpProperty(Residence property)
    {
        foreach (var energyConsumer in property._energyConsumers)
        {
            energyConsumer.PowerUp(energyConsumer.EnergyNeededToFunction);
        }
    }

    public override void Interact()
    {
        // Debug.Log("Using Property Manager");
        _interfaceUI.SetActive(true);
        GameEventsManager.Instance.OnToggleUi();
        Player.Instance.UseAltControls(true, this);
    }

    public void OpenInterfaceForLot(int id)
    {
        _activeProperty = _property.Find(x => x._propertyID == (PropertyID)id);
        DrawPropertyData(_activeProperty._data);

        ExtendedUIMono.ScaleOut(_apartmentsParentUI.transform, () =>
        {
            _apartmentsParentUI.SetActive(false);
            _activePropertyUI.SetActive(true);
        });
        // _apartmentsParentUI.SetActive(false);
        // _activePropertyUI.SetActive(true);
    }

    void DrawPropertyData(ResidenceConfigSO config)
    {
        _propertyName.text = config._residenceName;
        _maxResidents.text = config._maxResidents.ToString();
        _propertyRent.text = config._rent.ToString();
        _propertyEnergyCost.text = config._energyCost.ToString();
    }

    public void PayPowerBill()
    {
        Debug.Log("Paid Power bill");
        if (SocialCreditManager.Instance.CanBuy(_activeProperty._energyCost))
        {
            PowerUpProperty(_activeProperty);
            SocialCreditManager.Instance.Pay(_activeProperty._energyCost);
            if (IsQuestItem)
            {
                QuestItem questItem = GetComponent<QuestItem>();
                OnInteracted?.Invoke(questItem._questItemData._tag);
            }
        }

        else
        {
            Debug.Log("Not Enough Dinero Holmes");
        }
    }

    public void ClosePopup()
    {
        ExtendedUIMono.ScaleOut(ActivePopup.transform, () =>
        {
            ActivePopup.gameObject.SetActive(false);
            _propertyControlPanel.SetActive(true);
            ExtendedUIMono.ScaleIn(_propertyControlPanel.transform);
        });
    }
    public void ShowPopup(GameObject popup)
    {
        ExtendedUIMono.ScaleOut(_propertyControlPanel.transform, () =>
        {
            popup.transform.localScale = Vector3.zero;
            popup.SetActive(true);
            _propertyControlPanel.SetActive(false);
            ExtendedUIMono.ScaleIn(popup.transform);
        });
    }

    public void OnButtonPress(Inputs button)
    {
        throw new NotImplementedException();
    }


}
