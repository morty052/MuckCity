using System;
using TMPro;
using UnityEngine;

public struct PersistentSocialCreditData
{
    public int _socialCredit;
    public string _socialCreditTextValue;


    public PersistentSocialCreditData(int socialCredit, string socialCreditTextValue)
    {
        _socialCredit = socialCredit;
        _socialCreditTextValue = socialCreditTextValue;
    }

}

public class SocialCreditManager : MonoBehaviour, IHavePersistentData
{
    [field: SerializeField] public int SocialCredit { get; private set; }

    [SerializeField] private TextMeshProUGUI _socialCreditText;
    [SerializeField] private int _lastSavedCredit;


    public static SocialCreditManager Instance { get; private set; }
    public bool ShouldAutoSave { get => ShouldAutoSave; set => ShouldAutoSave = value; }
    public string SAVE_FILE_NAME { get => "SOCIALCREDIT"; set => SAVE_FILE_NAME = value; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadPersistentData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        SocialCredit = _lastSavedCredit;
        _socialCreditText.text = SocialCredit.ToString();

    }


    void OnEnable()
    {
        GameEventsManager.OnSocialCreditUpdated += OnSocialCreditUpdated;
        GameEventsManager.OnDeliveryPointReachedEvent += OnDeliveryPointReached;
        GameEventsManager.OnPurchaseItem += OnPurchase;
        // GameEventsManager.OnShouldAutoSave += TriggerAutoSave;
    }

    void OnDisable()
    {
        GameEventsManager.OnSocialCreditUpdated -= OnSocialCreditUpdated;
        GameEventsManager.OnDeliveryPointReachedEvent -= OnDeliveryPointReached;
        GameEventsManager.OnPurchaseItem -= OnPurchase;
        // GameEventsManager.OnShouldAutoSave -= TriggerAutoSave;

        // TriggerAutoSave();
    }

    private void OnPurchase(ShopItemSO sO)
    {
        SocialCredit -= sO._price;
        _socialCreditText.text = SocialCredit.ToString();
    }

    private void OnDeliveryPointReached(DeliveryData data)
    {
        SocialCredit += data._deliveryFee;
        _socialCreditText.text = SocialCredit.ToString();
        Debug.Log("Received delivery fee: " + data._deliveryFee + " social credit: " + SocialCredit);
    }

    public void TriggerAutoSave()
    {
        _lastSavedCredit = SocialCredit;
        PersistentSocialCreditData data = new()
        {
            _socialCredit = _lastSavedCredit,
            _socialCreditTextValue = _lastSavedCredit.ToString()
        };

        ES3.Save(SAVE_FILE_NAME, data);
        Debug.Log("saved social credit data: " + _lastSavedCredit);
    }

    // private void OnPurchase(ShopItemSO sO)
    // {
    //     SocialCredit -= sO.Price;
    //     _socialCreditText.text = SocialCredit.ToString();
    // }

    private void OnSocialCreditUpdated(int credit, bool isDeduction = false)
    {
        if (isDeduction)
        {
            SocialCredit -= credit;
        }

        else
        {
            SocialCredit += credit;
        }

        _socialCreditText.text = SocialCredit.ToString();
    }



    public void LoadPersistentData()
    {
        // PersistentSocialCreditData data = ES3.Load<PersistentSocialCreditData>(SAVE_FILE_NAME);
        // _lastSavedCredit = data._socialCredit;
        // Debug.Log("Loaded social credit data: " + _lastSavedCredit);
    }
}
