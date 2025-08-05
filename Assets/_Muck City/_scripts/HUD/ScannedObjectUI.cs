
using System;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public enum DiscoverableItem
{
    IFA_CRYSTAL = 0
}

public class ScannedObjectUI : MonoBehaviour, IDoQuickAction
{
    public static ScannedObjectUI Instance { get; private set; }
    [SerializeField] GameObject _scannedObjectUI;
    [SerializeField] TextMeshProUGUI _scannedObjectName;
    [SerializeField] TextMeshProUGUI _scannedObjectDescription;

    [SerializeField, TabGroup("Quick Preview")] GameObject _preview;
    [SerializeField, TabGroup("Quick Preview")] TextMeshProUGUI _previewNameText;

    [SerializeField, TabGroup("Scan UI")] Image _scanBarImage;
    [SerializeField, TabGroup("Scan UI")] GameObject _scanBar;

    [SerializeField, TabGroup("Scan Tv")] GameObject _scanTV;
    [SerializeField, TabGroup("Scan Tv")] TextMeshProUGUI _scanTVItemNameText;
    [SerializeField, TabGroup("Scan Tv")] TextMeshProUGUI _scanTVItemDescription;
    [SerializeField, TabGroup("Scan Tv")] Transform _scanTVItemUseParent;

    [ShowInInspector] public HashSet<ScanDetails> _discoverableItems = new();
    private bool _scanInProgress;

    [SerializeField, TabGroup("Debug")] IScannableObject _lastScannedItem;
    public VisualEffect _harvestVFX;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _scannedObjectUI.SetActive(false);
            _preview.SetActive(false);
            _scanBar.transform.localScale = Vector3.zero;
            _scanBarImage.fillAmount = 0;
            HideScanTv();
            LoadPersistentData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnDisable()
    {
        AutoSave();
    }

    void AutoSave()
    {
        ES3.Save("DISCOVERED_ITEMS", _discoverableItems);
    }

    void LoadPersistentData()
    {
        HashSet<ScanDetails> discoveredItems = ES3.Load("DISCOVERED_ITEMS", new HashSet<ScanDetails>());
        if (discoveredItems == null) return;
        _discoverableItems = discoveredItems;
    }

    public void SetDetails(string name, string description)
    {
        _scannedObjectName.text = name;
        _scannedObjectDescription.text = description;
        _previewNameText.text = name;

        _scanTVItemDescription.text = description;
        _scanTVItemNameText.text = name;
    }

    public void OnScanObject(ScanDetails scanDetails)
    {
        HideScanBar();
        SetDetails(scanDetails._scanName, scanDetails._scanDescription);
        _preview.SetActive(true);
        Player.Instance._activeQuickAction = this;
        Invoke(nameof(ResetQuickAction), 3f);
        DocumentItem(scanDetails);
        ShowScanTv();
    }

    void ResetQuickAction()
    {
        _preview.SetActive(false);
        Player.Instance._activeQuickAction = null;
    }

    void DocumentItem(ScanDetails scanDetails)
    {
        _discoverableItems.Add(scanDetails);
        AutoSave();
    }



    public void DoQuickAction()
    {
        _preview.SetActive(false);
        _scannedObjectUI.SetActive(true);
    }

    public void ProgressScan(float progress, Transform transform)
    {
        if (!_scanInProgress)
        {
            _scanBar.transform.SetParent(transform);
            _scanBar.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            _scanBar.transform.DOScale(new Vector3(0.01f, 0.01f, 0.01f), 0.3f);
            _scanInProgress = true;
        }
        _scanBarImage.fillAmount = progress;
        if (progress >= 1)
        {
            HideScanBar();
            _lastScannedItem = transform.GetComponent<IScannableObject>();
        }
    }

    void ShowScanTv()
    {
        _scanTV.transform.SetParent(_lastScannedItem.GameObject.transform);
        _scanTV.SetActive(true);
        _scanTV.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        _scanTV.transform.DOScale(new Vector3(0.01f, 0.01f, 0.01f), 0.3f);
    }
    void HideScanTv()
    {

        _scanTV.transform.DOScale(Vector3.zero, 0.3f).onComplete = () => { _scanTV.SetActive(false); };
        _scanTV.transform.SetParent(null);
    }


    public void HideScanBar()
    {
        _scanBar.transform.SetParent(null);
        _scanBarImage.fillAmount = 0;
        _scanBar.transform.DOScale(Vector3.zero, 0.3f);
        _scanInProgress = false;
    }


}
