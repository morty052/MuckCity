using TMPro;
using UnityEngine;

public class ScannedObjectUI : MonoBehaviour, IDoQuickAction
{
    public static ScannedObjectUI Instance { get; private set; }
    [SerializeField] GameObject _scannedObjectUI;
    [SerializeField] TextMeshProUGUI _scannedObjectName;
    [SerializeField] TextMeshProUGUI _scannedObjectDescription;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _scannedObjectUI.SetActive(false);
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
    }

    public void OnScanObject(ScanDetails scanDetails)
    {
        SetDetails(scanDetails._scanName, scanDetails._scanDescription);
        Player.Instance._activeQuickAction = this;
        Invoke(nameof(ResetQuickAction), 3f);
    }

    void ResetQuickAction()
    {
        Player.Instance._activeQuickAction = null;
    }

    public void DoQuickAction()
    {
        _scannedObjectUI.SetActive(true);
    }
}
