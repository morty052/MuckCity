
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class ScannedObjectUI : MonoBehaviour, IDoQuickAction
{
    public static ScannedObjectUI Instance { get; private set; }
    [SerializeField] GameObject _scannedObjectUI;
    [SerializeField] TextMeshProUGUI _scannedObjectName;
    [SerializeField] TextMeshProUGUI _scannedObjectDescription;

    [SerializeField, TabGroup("Quick Preview")] GameObject _preview;
    [SerializeField, TabGroup("Quick Preview")] TextMeshProUGUI _previewNameText;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _scannedObjectUI.SetActive(false);
            _preview.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetDetails(string name, string description)
    {
        _scannedObjectName.text = name;
        _scannedObjectDescription.text = description;
        _previewNameText.text = name;
    }

    public void OnScanObject(ScanDetails scanDetails)
    {
        SetDetails(scanDetails._scanName, scanDetails._scanDescription);
        _preview.SetActive(true);
        Player.Instance._activeQuickAction = this;
        Invoke(nameof(ResetQuickAction), 3f);
    }

    void ResetQuickAction()
    {
        _preview.SetActive(false);
        Player.Instance._activeQuickAction = null;
    }

    public void DoQuickAction()
    {
        _preview.SetActive(false);
        _scannedObjectUI.SetActive(true);
    }
}
