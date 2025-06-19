using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;



public class SocialCreditManager : MonoBehaviour, IHavePersistentData
{
    [field: SerializeField] public int SocialCredit { get; private set; }

    [SerializeField] private TextMeshProUGUI _socialCreditText;
    [SerializeField] private int _lastSavedCredit;


    public static SocialCreditManager Instance { get; private set; }
    public bool ShouldAutoSave { get => AutoSaveManager.ShouldAutoSave(SaveAble.CREDITS); }

    public SaveAble SAVE_ID => SaveAble.CREDITS;

    private SocialCreditData _socialCreditData;

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
        if (_socialCreditData.Equals(null)) return;
        SocialCredit = _socialCreditData._socialCredit;
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
        object data = AutoSaveManager.Load(SAVE_ID);
        if (data.IsUnityNull())
        {
            Debug.Log("Social credit not loaded" + _lastSavedCredit);
            return;
        }
        _socialCreditData = (SocialCreditData)data;
        Debug.Log("Loaded social credit data: " + _lastSavedCredit);
    }

    public void AutoSave()
    {
        _lastSavedCredit = SocialCredit;
        SocialCreditData data = new()
        {
            _socialCredit = _lastSavedCredit,
            _socialCreditTextValue = _lastSavedCredit.ToString()
        };

        AutoSaveManager.Autosave(SAVE_ID, data);
        Debug.Log("saved social credit data: " + _lastSavedCredit);
    }
}
